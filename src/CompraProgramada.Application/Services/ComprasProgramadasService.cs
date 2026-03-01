using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Implementação do serviço de compras programadas
/// Orquestra todo o processo de compra nas datas programadas (5, 15, 25 de cada mês)
/// </summary>
public class ComprasProgramadasService : IComprasProgramadasService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICotacaoService _cotacaoService;
    private readonly IKafkaProducerService _kafkaProducer;
    private const decimal TaxaIR = 0.00005m; // 0.005%
    private const int LotePadrao = 100;

    public ComprasProgramadasService(
        IUnitOfWork unitOfWork,
        ICotacaoService cotacaoService,
        IKafkaProducerService kafkaProducer)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cotacaoService = cotacaoService ?? throw new ArgumentNullException(nameof(cotacaoService));
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
    }

    public async Task<int> ProcessarComprasAgrupadasAsync(DateTime dataCompra)
    {
        if (!EhDataCompraValida(dataCompra))
            throw new ArgumentException($"Data inválida: {dataCompra:dd/MM/yyyy}. Deve ser 5, 15 ou 25");

        var dataAjustada = AjustarDataParaDiaUtil(dataCompra);

        var ordensProcessadas = 0;

        try
        {
            // Buscar clientes ativos com cesta
            var todosClientes = await _unitOfWork.ClienteRepository.ObterTodosAsync();
            var clientes = todosClientes
                .Where(c => c.Ativo && c.CestaRecomendacaoId.HasValue)
                .ToList();
            
            if (!clientes.Any())
                return 0;

            // Agrupar por cesta
            var gruposPorCesta = clientes
                .GroupBy(c => c.CestaRecomendacaoId.Value)
                .ToList();

            // Obter conta master
            var contaMaster = await ObterContaMasterAsync();
            if (contaMaster == null)
                return 0;

            foreach (var grupoCesta in gruposPorCesta)
            {
                var cestaId = grupoCesta.Key;
                var cesta = await _unitOfWork.CestaRepository.ObterPorIdAsync(cestaId);
                
                if (cesta == null || !cesta.Ativa)
                    continue;

                var grupoClientes = grupoCesta.ToList();
                var valorTotalConsolidado = grupoClientes.Sum(c => c.CalcularValorParcela());
                
                // Processar cada ativo da cesta
                foreach (var item in cesta.Itens)
                {
                    var tickerStr = item.Ticker.Valor;
                    var cotacao = await _cotacaoService.ObterCotacaoPorDataAsync(tickerStr, dataAjustada);
                    
                    if (cotacao == null)
                        continue;

                    var valorAlocar = valorTotalConsolidado * (item.Percentual.Valor / 100m);
                    CalcularQuantidades(valorAlocar, cotacao.PrecoFechamento, LotePadrao, 
                        out long qty_padrao, out long qty_fracionaria);

                    if (qty_padrao > 0)
                    {
                        var ordem = new OrdemCompra(contaMaster.Id, item.Ticker, 
                            new Quantidade((int)qty_padrao), cotacao.PrecoFechamento, TipoMercado.LotePadrao);
                        await _unitOfWork.OrdemCompraRepository.AdicionarAsync(ordem);
                        ordensProcessadas++;
                    }

                    if (qty_fracionaria > 0)
                    {
                        var ordem = new OrdemCompra(contaMaster.Id, item.Ticker, 
                            new Quantidade((int)qty_fracionaria), cotacao.PrecoFechamento, TipoMercado.Fracionario);
                        await _unitOfWork.OrdemCompraRepository.AdicionarAsync(ordem);
                        ordensProcessadas++;
                    }

                    var valorIR = (qty_padrao + qty_fracionaria) * (decimal)cotacao.PrecoFechamento * TaxaIR;
                    await PublicarIRAsync(contaMaster.Id, tickerStr, dataCompra, valorIR);
                }

                await DistribuirParaFilhotesAsync(contaMaster.Id, grupoClientes, cesta, dataAjustada);
            }

            await _unitOfWork.SaveChangesAsync();
            return ordensProcessadas;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao processar compras de {dataCompra:dd/MM/yyyy}", ex);
        }
    }

    public async Task<List<DateTime>> ObterProximasDataasCompraAsync()
    {
        var hoje = DateTime.Today;
        var proximas = new List<DateTime>();
        var diaCompra = 5;

        while (proximas.Count < 3 && diaCompra <= 25)
        {
            try
            {
                var data = new DateTime(hoje.Year, hoje.Month, diaCompra);
                if (data >= hoje)
                    proximas.Add(data);
            }
            catch { }

            diaCompra = diaCompra == 5 ? 15 : (diaCompra == 15 ? 25 : 35);
        }

        if (proximas.Count < 3)
        {
            var proximoMes = hoje.AddMonths(1);
            diaCompra = 5;
            while (proximas.Count < 3 && diaCompra <= 25)
            {
                try
                {
                    var data = new DateTime(proximoMes.Year, proximoMes.Month, diaCompra);
                    proximas.Add(data);
                }
                catch { }

                diaCompra = diaCompra == 5 ? 15 : 25;
            }
        }

        return proximas.OrderBy(d => d).ToList();
    }

    public async Task<bool> HaComprasPendentesAsync()
    {
        var proximas = await ObterProximasDataasCompraAsync();
        return proximas.Any();
    }

    /// <summary>
    /// Valida se a data é um dia de compra programada (5, 15 ou 25)
    /// </summary>
    private bool EhDataCompraValida(DateTime data) => data.Day == 5 || data.Day == 15 || data.Day == 25;

    /// <summary>
    /// Ajusta a data para o próximo dia útil se cair em sábado/domingo
    /// RN-026: Se o dia de compra (5, 15, 25) cair em fim de semana, processar na segunda-feira
    /// </summary>
    public DateTime AjustarDataParaDiaUtil(DateTime data)
    {
        // Se for sábado (6), avança para segunda (2 dias)
        if (data.DayOfWeek == DayOfWeek.Saturday)
            return data.AddDays(2);

        // Se for domingo (0), avança para segunda (1 dia)
        if (data.DayOfWeek == DayOfWeek.Sunday)
            return data.AddDays(1);

        // Caso contrário, mantém a data original
        return data;
    }

    private void CalcularQuantidades(decimal valor, decimal preco, int lote, out long padrao, out long fracao)
    {
        padrao = 0;
        fracao = 0;
        if (preco <= 0) return;

        var total = (long)Math.Floor(valor / preco);
        padrao = (total / lote) * lote;
        fracao = total % lote;
    }

    private async Task<ContaGrafica?> ObterContaMasterAsync()
    {
        var contas = await _unitOfWork.ContaGraficaRepository.ObterTodosAsync();
        return contas.FirstOrDefault(c => c.Tipo == TipoConta.Master);
    }

    private async Task DistribuirParaFilhotesAsync(long masterId, List<Cliente> clientes, CestaRecomendacao cesta, DateTime data)
    {
        var valorTotal = clientes.Sum(c => c.CalcularValorParcela());
        if (valorTotal <= 0) return;

        foreach (var cliente in clientes)
        {
            var proporcao = cliente.CalcularValorParcela() / valorTotal;

            foreach (var item in cesta.Itens)
            {
                var qty = (long)Math.Floor(proporcao * 100);
                if (qty > 0)
                {
                    var dist = new Distribuicao(cliente.Id, masterId, item.Ticker, 
                        new Quantidade((int)qty), data);
                    await _unitOfWork.DistribuicaoRepository.AdicionarAsync(dist);
                }
            }
        }
    }

    private async Task PublicarIRAsync(long masterId, string ticker, DateTime data, decimal valor)
    {
        var msg = new { masterId, ticker, data = data.ToString("yyyy-MM-dd"), valor, percentual = "0.005%" };
        await _kafkaProducer.PublicarAsync("compra-programada-ir", masterId.ToString(), msg);
    }
}
