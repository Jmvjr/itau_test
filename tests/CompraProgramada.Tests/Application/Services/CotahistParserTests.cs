using Xunit;
using CompraProgramada.Application.Services;

namespace CompraProgramada.Tests.Application.Services;

/// <summary>
/// Testes para o parser COTAHIST
/// </summary>
public class CotahistParserTests
{
    private readonly string _pastaTesteCotacoes = Path.Combine(
        Directory.GetCurrentDirectory(), 
        "..", "..", "..", "..", "..", 
        "cotacoes");

    [Fact]
    public void ParsearArquivoMensal_DeveLerCotacoesValidas()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);

        // Assert
        Assert.NotNull(cotacoes);
        Assert.NotEmpty(cotacoes);
        Assert.True(cotacoes.Count > 100, "Esperava pelo menos 100 cotacoes no arquivo mensal");

        // Verificar primeira cotação (MRSA3B) - primeiros dados do arquivo
        var mrsa3b = cotacoes.FirstOrDefault(c => c.Ticker == "MRSA3B");
        Assert.NotNull(mrsa3b);
        Assert.Equal(new DateTime(2026, 02, 02), mrsa3b.DataPregao);
        Assert.Equal(42.80m, mrsa3b.PrecoFechamento);
        Assert.Equal(42.80m, mrsa3b.PrecoAbertura);
        Assert.Equal(42.80m, mrsa3b.PrecoMaximo);
        Assert.Equal(42.80m, mrsa3b.PrecoMinimo);
    }

    [Fact]
    public void ParsearArquivoMensal_DeveExtrairTickersValidos()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);
        var tickers = cotacoes.Select(c => c.Ticker).Distinct().ToList();

        // Assert
        Assert.NotEmpty(tickers);
        Assert.Contains("MRSA3B", tickers);  // Primeiro ticker do arquivo
        
        // Validar que todos os tickers têm tamanho apropriado
        foreach (var ticker in tickers)
        {
            Assert.True(ticker.Length >= 4 && ticker.Length <= 6, 
                $"Ticker '{ticker}' deve ter 4-6 caracteres");
        }
    }

    [Fact]
    public void ParsearArquivoMensal_DevePreencherTodosCampos()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);
        var mrsa3b = cotacoes.FirstOrDefault(c => c.Ticker == "MRSA3B");

        // Assert
        Assert.NotNull(mrsa3b);
        Assert.Equal("MRSA3B", mrsa3b.Ticker);
        Assert.Equal(new DateTime(2026, 02, 02), mrsa3b.DataPregao);
        Assert.True(mrsa3b.QuantidadeMovimentada > 0);
        Assert.True(mrsa3b.VolumeMovimentado > 0);
    }



    [Fact]
    public void ParsearArquivo_ComCaminhoInvalido_DeveLancarException()
    {
        // Arrange
        var caminhoInvalido = "/path/que/nao/existe/COTAHIST_D20260225.TXT";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => CotahistParser.ParsearArquivo(caminhoInvalido));
    }

    [Fact]
    public void ParsearArquivo_DeveRetornarListaNaoVazia()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);

        // Assert
        Assert.NotNull(cotacoes);
        Assert.NotEmpty(cotacoes);
    }

    [Fact]
    public void ParsearArquivo_TodosOsItemsDevemTerTickerValido()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);

        // Assert
        foreach (var cotacao in cotacoes)
        {
            Assert.NotNull(cotacao.Ticker);
            Assert.NotEmpty(cotacao.Ticker);
            Assert.True(cotacao.Ticker.All(c => char.IsLetterOrDigit(c)), 
                $"Ticker '{cotacao.Ticker}' contém caracteres inválidos");
            Assert.InRange(cotacao.Ticker.Length, 4, 6);
        }
    }

    [Fact]
    public void ParsearArquivo_TodosOsPrecosDeveraoSerPositivos()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);

        // Assert
        foreach (var cotacao in cotacoes)
        {
            Assert.True(cotacao.PrecoAbertura > 0, $"Preço de abertura inválido para {cotacao.Ticker}");
            Assert.True(cotacao.PrecoFechamento > 0, $"Preço de fechamento inválido para {cotacao.Ticker}");
            Assert.True(cotacao.PrecoMaximo > 0, $"Preço máximo inválido para {cotacao.Ticker}");
            Assert.True(cotacao.PrecoMinimo > 0, $"Preço mínimo inválido para {cotacao.Ticker}");
            Assert.True(cotacao.PrecoMinimo <= cotacao.PrecoMaximo, 
                $"Preço mínimo maior que máximo para {cotacao.Ticker}");
        }
    }

    [Fact]
    public void ParsearArquivoMensal_DeveLerCorretamente()
    {
        // Arrange
        var caminhoArquivo = Path.Combine(_pastaTesteCotacoes, "COTAHIST_M022026.TXT");

        // Act
        var cotacoes = CotahistParser.ParsearArquivo(caminhoArquivo);

        // Assert
        Assert.NotNull(cotacoes);
        Assert.NotEmpty(cotacoes);
        Assert.True(cotacoes.All(c => !string.IsNullOrEmpty(c.Ticker)));
    }
}
