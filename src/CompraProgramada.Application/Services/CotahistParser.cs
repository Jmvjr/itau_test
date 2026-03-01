namespace CompraProgramada.Application.Services;

/// <summary>
/// DTO para representar uma cotação do arquivo COTAHIST
/// </summary>
public class CotacaoHistoricoDTO
{
    public string Ticker { get; set; } = string.Empty;
    public DateTime DataPregao { get; set; }
    public decimal PrecoAbertura { get; set; }
    public decimal PrecoMaximo { get; set; }
    public decimal PrecoMinimo { get; set; }
    public decimal PrecoMedio { get; set; }
    public decimal PrecoFechamento { get; set; }
    public long QuantidadeMovimentada { get; set; }
    public decimal VolumeMovimentado { get; set; }
}

/// <summary>
/// Parser para arquivos COTAHIST do padrão B3
/// 
/// Especificação: https://www.b3.com.br/pt_br/market-data-e-indices/servicos-de-dados/market-data/
/// 
/// Formato posicional (fixed-width):
/// - Registro tipo 00: Header
/// - Registro tipo 01: Dados de cotação
/// - Registro tipo 99: Trailer
/// </summary>
public class CotahistParser
{
    /// <summary>
    /// Parsea um arquivo COTAHIST e retorna lista de cotações
    /// </summary>
    public static List<CotacaoHistoricoDTO> ParsearArquivo(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
            throw new FileNotFoundException($"Arquivo COTAHIST não encontrado: {caminhoArquivo}");

        var cotacoes = new List<CotacaoHistoricoDTO>();
        var linhas = File.ReadAllLines(caminhoArquivo);

        foreach (var linha in linhas)
        {
            // Verificar tipo de registro
            if (linha.Length < 2)
                continue;

            var tipoRegistro = linha.Substring(0, 2);

            if (tipoRegistro == "01")
            {
                var cotacao = ParsearLinhaRegistro01(linha);
                if (cotacao != null)
                    cotacoes.Add(cotacao);
            }
            // Ignorar headers (00) e trailers (99)
        }

        return cotacoes;
    }

    /// <summary>
    /// Parsea uma linha de registro tipo 01 (dados de cotação)
    /// 
    /// Layout posicional B3 COTAHIST (referência oficial B3):
    /// Pos 1-2:       Tipo de registro
    /// Pos 3-10:      Data (YYYYMMDD)
    /// Pos 11-12:     Código BDI (02=Lote Padrão, 96=Fracionário)
    /// Pos 13-24:     Código do ativo (Ticker)
    /// Pos 25-27:     Tipo de mercado (010=À Vista, 020=Fracionário)
    /// Pos 28-39:     Nome resumido
    /// Pos 40-49:     Especificação
    /// Pos 50-52:     Prazo
    /// Pos 53-56:     Moeda
    /// Pos 57-69:     Preço de abertura (13 chars, N(11,2))
    /// Pos 70-82:     Preço máximo (13 chars, N(11,2))
    /// Pos 83-95:     Preço mínimo (13 chars, N(11,2))
    /// Pos 96-108:    Preço médio (13 chars, N(11,2))
    /// Pos 109-121:   Preço de fechamento (13 chars, N(11,2)) 
    /// Pos 153-170:   Quantidade movimentada (18 chars)
    /// Pos 171-188:   Volume movimentado (18 chars, N(16,2))
    /// </summary>
    private static CotacaoHistoricoDTO? ParsearLinhaRegistro01(string linha)
    {
        try
        {
            // Validar tamanho mínimo (arquivo tem 245 caracteres)
            if (linha.Length < 188)
                return null;

            // Extrair campos por posição (B3 é 1-indexed, convertendo para 0-indexed)
            var codbdi = linha.Substring(10, 2);        // Pos 11-12: Código BDI
            var tpmerc = linha.Substring(24, 3);        // Pos 25-27: Tipo de mercado
            
            // Filtrar apenas lote padrão (02) e fracionário (96)
            if (codbdi != "02" && codbdi != "96")
                return null;
            
            // Filtrar apenas à vista (010) e fracionário (020)
            if (tpmerc != "010" && tpmerc != "020")
                return null;

            var dataStr = linha.Substring(2, 8);              // Pos 3-10: Data
            var ticker = linha.Substring(12, 12).Trim();     // Pos 13-24: Ticker
            var precoAberturaStr = linha.Substring(56, 13);  // Pos 57-69: Preço Abertura
            var precoMaximoStr = linha.Substring(69, 13);    // Pos 70-82: Preço Máximo
            var precoMinimoStr = linha.Substring(82, 13);    // Pos 83-95: Preço Mínimo
            var precoMedioStr = linha.Substring(95, 13);     // Pos 96-108: Preço Médio
            var precoFechamentoStr = linha.Substring(108, 13); // Pos 109-121: Preço Fechamento
            var qtdMovimentadaStr = linha.Substring(152, 18); // Pos 153-170: Quantidade
            var volumeStr = linha.Substring(170, 18);        // Pos 171-188: Volume

            // Parsear data
            if (!DateTime.TryParseExact(dataStr, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var dataPregao))
                return null;

            // Filtrar tickers válidos (excluir todos que contenham caracteres inválidos ou estejam vazios)
            if (string.IsNullOrWhiteSpace(ticker) || !EhTickerValido(ticker))
                return null;

            // Converter preços (formato: 8 dígitos com 2 casas decimais)
            // Exemplo: "00000356" = 3.56
            var precoAbertura = ConvertPreco(precoAberturaStr);
            var precoMaximo = ConvertPreco(precoMaximoStr);
            var precoMinimo = ConvertPreco(precoMinimoStr);
            var precoMedio = ConvertPreco(precoMedioStr);
            var precoFechamento = ConvertPreco(precoFechamentoStr);

            // Converter quantidade e volume
            var qtdMovimentada = long.TryParse(qtdMovimentadaStr.Trim(), out var qtd) ? qtd : 0;
            var volume = ConvertPreco(volumeStr);

            return new CotacaoHistoricoDTO
            {
                Ticker = ticker,
                DataPregao = dataPregao,
                PrecoAbertura = precoAbertura,
                PrecoMaximo = precoMaximo,
                PrecoMinimo = precoMinimo,
                PrecoMedio = precoMedio,
                PrecoFechamento = precoFechamento,
                QuantidadeMovimentada = qtdMovimentada,
                VolumeMovimentado = volume
            };
        }
        catch
        {
            // Ignorar linhas com erro ao parsear
            return null;
        }
    }

    /// <summary>
    /// Converte string de preço (8 dígitos com 2 casas decimais implícitas)
    /// Exemplo: "00000356" = 3.56
    /// </summary>
    private static decimal ConvertPreco(string precoStr)
    {
        if (string.IsNullOrWhiteSpace(precoStr))
            return 0;

        precoStr = precoStr.Trim();
        
        if (!long.TryParse(precoStr, out var precoInteiro))
            return 0;

        // Dividir por 100 para considerar as 2 casas decimais implícitas
        return precoInteiro / 100m;
    }

    /// <summary>
    /// Valida se é um ticker válido (4-6 caracteres, apenas letras e números)
    /// </summary>
    private static bool EhTickerValido(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            return false;

        // Ticker deve ter entre 4 e 6 caracteres
        if (ticker.Length < 4 || ticker.Length > 6)
            return false;

        // Apenas letras e números
        return ticker.All(c => char.IsLetterOrDigit(c));
    }
}
