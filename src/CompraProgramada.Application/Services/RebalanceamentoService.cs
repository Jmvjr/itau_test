using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Domain.ValueObjects;

namespace CompraProgramada.Application.Services;

/// <summary>
/// Implementação do serviço de rebalanceamento de carteira
/// RN-019: Disparado quando a cesta de recomendação muda
/// </summary>
public class RebalanceamentoService : IRebalanceamentoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICotacaoService _cotacaoService;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ICustodiaRepository _custodiaRepository;

    private const decimal AliquotaIR = 0.20m; // 20% para vendas > 20.000
    private const decimal LimiteIsencaoIR = 20000m;

    public RebalanceamentoService(
        IUnitOfWork unitOfWork,
        ICotacaoService cotacaoService,
        IKafkaProducerService kafkaProducer,
        ICustodiaRepository custodiaRepository)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cotacaoService = cotacaoService ?? throw new ArgumentNullException(nameof(cotacaoService));
        _kafkaProducer = kafkaProducer ?? throw new ArgumentNullException(nameof(kafkaProducer));
        _custodiaRepository = custodiaRepository ?? throw new ArgumentNullException(nameof(custodiaRepository));
    }

    public async Task<int> ProcessarRebalanceamentoAsync(CestaRecomendacao cestaAnterior, CestaRecomendacao novaCesta)
    {
        // Identificar tickers que mudaram
        var tickersSaidos = novaCesta.IdentificarTickersSaidos(cestaAnterior);
        var tickersEntrados = novaCesta.IdentificarTickersEntrados(cestaAnterior);

        // Se não há alterações, nada a fazer
        if (tickersSaidos.Count == 0 && tickersEntrados.Count == 0)
            return 0;

        var rebalanceamentosProcessados = 0;

        try
        {
            // Obter todos os clientes ativos com cesta
            var clientes = await _unitOfWork.ClienteRepository.ObterTodosAsync();
            var clientesAtivos = clientes
                .Where(c => c.Ativo && c.CestaRecomendacaoId.HasValue)
                .ToList();

        if (!clientesAtivos.Any())
            return 0;

        // Processar cada cliente
        foreach (var cliente in clientesAtivos)
        {
            var detalhes = await CalcularAjustesAsync(cliente, cestaAnterior, novaCesta);
                // Executar vendas (tickers saídos)
                foreach (var (ticker, quantidade) in detalhes.TickersParaVender)
                {
                    await ExecutarVendaAsync(cliente, ticker, quantidade, detalhes);
                }

                // Executar compras (tickers entrados)
                foreach (var (ticker, quantidade) in detalhes.TickersParaComprar)
                {
                    await ExecutarCompraAsync(cliente, ticker, quantidade);
                }

                // Executar rebalanceamento (proporção mudou)
                foreach (var (ticker, novaQuantidade) in detalhes.TickersParaRebalancear)
                {
                    await ExecutarRebalanceamentoAsync(cliente, ticker, novaQuantidade, novaCesta);
                }

                rebalanceamentosProcessados++;
            }

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao processar rebalanceamento: {ex.Message}", ex);
        }

        return rebalanceamentosProcessados;
    }

    public async Task<DetalhesRebalanceamento> CalcularAjustesAsync(
        Cliente cliente,
        CestaRecomendacao cestaAnterior,
        CestaRecomendacao novaCesta)
    {
        var detalhes = new DetalhesRebalanceamento();
        var data = DateTime.UtcNow;

        // Obter custodia atual do cliente
        var custodias = await _custodiaRepository.ObterTodosPorClienteAsync(cliente.Id);
        var custodiasAtivas = custodias
            .GroupBy(c => c.Ticker.Valor)
            .ToDictionary(g => g.Key, g => g.Sum(c => c.Quantidade.Valor));

        // Obter cotações necessárias
        var cotacoes = new Dictionary<string, decimal>();
        foreach (var item in cestaAnterior.Itens.Union(novaCesta.Itens))
        {
            if (!cotacoes.ContainsKey(item.Ticker.Valor))
            {
                var cotacao = await _cotacaoService.ObterCotacaoPorDataAsync(
                    item.Ticker.Valor, data);
                if (cotacao != null)
                    cotacoes[item.Ticker.Valor] = cotacao.PrecoFechamento;
            }
        }

        // 1. Identificar tickers que saíram
        var tickersSaidos = novaCesta.IdentificarTickersSaidos(cestaAnterior);
        foreach (var ticker in tickersSaidos)
        {
            if (custodiasAtivas.TryGetValue(ticker.Valor, out var quantidade) && quantidade > 0)
            {
                detalhes.TickersParaVender[ticker] = quantidade;

                // Calcular valor da venda para IR
                if (cotacoes.TryGetValue(ticker.Valor, out var precoFechamento))
                {
                    detalhes.ValorTotalVendas += quantidade * precoFechamento;
                }
            }
        }

        // 2. Identificar tickers que entraram
        var tickersEntrados = novaCesta.IdentificarTickersEntrados(cestaAnterior);
        foreach (var ticker in tickersEntrados)
        {
            // Calcular quantidade a comprar baseado no aporte do cliente e percentual na nova cesta
            var percentualNova = novaCesta.ObterPercentualTicker(ticker);
            if (percentualNova != null)
            {
                var valorAporte = cliente.CalcularValorParcela();
                var valorTicker = valorAporte * (percentualNova.Valor / 100m);

                if (cotacoes.TryGetValue(ticker.Valor, out var precoFechamento))
                {
                    var quantidade = (int)Math.Floor(valorTicker / precoFechamento);
                    if (quantidade > 0)
                    {
                        detalhes.TickersParaComprar[ticker] = quantidade;
                    }
                }
            }
        }

        // 3. Identificar tickers com proporção alterada
        var tickersComAlteracao = cestaAnterior.Itens
            .Where(i => !tickersSaidos.Contains(i.Ticker) && !tickersEntrados.Any(t => t == i.Ticker))
            .Select(i => i.Ticker)
            .ToList();

        foreach (var ticker in tickersComAlteracao)
        {
            var percentualAntigo = cestaAnterior.ObterPercentualTicker(ticker);
            var percentualNovo = novaCesta.ObterPercentualTicker(ticker);

            if (percentualAntigo != null && percentualNovo != null && percentualAntigo.Valor != percentualNovo.Valor)
            {
                var valorAporte = cliente.CalcularValorParcela();
                var novoValor = valorAporte * (percentualNovo.Valor / 100m);

                if (cotacoes.TryGetValue(ticker.Valor, out var precoFechamento))
                {
                    var novaQuantidade = (int)Math.Floor(novoValor / precoFechamento);
                    var quantidadeAtual = custodiasAtivas.ContainsKey(ticker.Valor) 
                        ? custodiasAtivas[ticker.Valor] 
                        : 0;

                    if (novaQuantidade > quantidadeAtual)
                    {
                        detalhes.TickersParaComprar[ticker] = novaQuantidade - quantidadeAtual;
                    }
                    else if (novaQuantidade < quantidadeAtual)
                    {
                        detalhes.TickersParaVender[ticker] = quantidadeAtual - novaQuantidade;
                        detalhes.ValorTotalVendas += (quantidadeAtual - novaQuantidade) * precoFechamento;
                    }
                }
            }
        }

        // 4. Calcular IR (20% se vendas > R$ 20.000)
        if (detalhes.ValorTotalVendas > LimiteIsencaoIR)
        {
            detalhes.IRAplicado = detalhes.ValorTotalVendas * AliquotaIR;
        }

        return detalhes;
    }

    private async Task ExecutarVendaAsync(
        Cliente cliente,
        Ticker ticker,
        int quantidade,
        DetalhesRebalanceamento detalhes)
    {
        // Vender posição do cliente para este ticker
        var custodias = await _custodiaRepository.ObterTodosPorClienteAsync(cliente.Id);
        var custodiaVenda = custodias
            .Where(c => c.Ticker.Valor == ticker.Valor)
            .FirstOrDefault();

        if (custodiaVenda != null && custodiaVenda.Quantidade.Valor >= quantidade)
        {
            custodiaVenda.Vender(new Quantidade(quantidade));
        }

        // Publicar evento de venda (IR será calculado aqui)
        await _kafkaProducer.PublicarAsync("rebalanceamento-vendas", cliente.CPF.Valor, new
        {
            clienteId = cliente.Id,
            ticker = ticker.Valor,
            quantidade,
            irAplicado = detalhes.IRAplicado,
            dataVenda = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });
    }

    private async Task ExecutarCompraAsync(Cliente cliente, Ticker ticker, int quantidade)
    {
        // Criar ou atualizar custodia do cliente
        var custodias = await _custodiaRepository.ObterTodosPorClienteAsync(cliente.Id);
        var custodiaExistente = custodias
            .Where(c => c.Ticker.Valor == ticker.Valor)
            .FirstOrDefault();

        if (custodiaExistente != null)
        {
            // Atualizar quantidade e preço médio
            custodiaExistente.AtualizarComNovaCompra(new Quantidade(quantidade), 0m);
        }
        else
        {
            // Criar nova custodia
            var novaCustodia = new Custodia(
                cliente.Id,
                ticker,
                new Quantidade(quantidade),
                0m); // Preço inicial será 0, será atualizado com custodia master

            await _custodiaRepository.AdicionarAsync(novaCustodia);
        }

        // Publicar evento de compra
        await _kafkaProducer.PublicarAsync("rebalanceamento-compras", cliente.CPF.Valor, new
        {
            clienteId = cliente.Id,
            ticker = ticker.Valor,
            quantidade,
            dataCompra = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });
    }

    private async Task ExecutarRebalanceamentoAsync(
        Cliente cliente,
        Ticker ticker,
        int novaQuantidade,
        CestaRecomendacao novaCesta)
    {
        var custodias = await _custodiaRepository.ObterTodosPorClienteAsync(cliente.Id);
        var custodiaExistente = custodias
            .Where(c => c.Ticker.Valor == ticker.Valor)
            .FirstOrDefault();

        if (custodiaExistente != null)
        {
            var quantidadeAtual = custodiaExistente.Quantidade.Valor;
            if (novaQuantidade > quantidadeAtual)
            {
                // Comprar mais
                var diferenca = novaQuantidade - quantidadeAtual;
                custodiaExistente.AtualizarComNovaCompra(new Quantidade(diferenca), custodiaExistente.PrecoMedio);
            }
            else if (novaQuantidade < quantidadeAtual)
            {
                // Vender parte
                var diferenca = quantidadeAtual - novaQuantidade;
                custodiaExistente.Vender(new Quantidade(diferenca));
            }
        }

        // Publicar evento de rebalanceamento
        await _kafkaProducer.PublicarAsync("rebalanceamento-ajustes", cliente.CPF.Valor, new
        {
            clienteId = cliente.Id,
            ticker = ticker.Valor,
            novaQuantidade,
            novoPercentual = novaCesta.ObterPercentualTicker(ticker)?.Valor,
            dataRebalanceamento = DateTime.UtcNow.ToString("yyyy-MM-dd")
        });
    }
}
