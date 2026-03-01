using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.ValueObjects;

public class PercentualTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(25.5)]
    [InlineData(99.99)]
    public void Deve_CriarPercentual_ComValorValido(decimal valor)
    {
        // Arrange & Act
        var percentual = new Percentual(valor);

        // Assert
        percentual.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    [InlineData(101)]
    [InlineData(150)]
    public void Deve_Rejeitar_ValorForaDoIntervalo(decimal valor)
    {
        // Arrange & Act & Assert
        var act = () => new Percentual(valor);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*0%*100%*");
    }

    [Fact]
    public void ParaDecimal_DeveConverterCorretamente()
    {
        // Arrange
        var percentual = new Percentual(30);

        // Act
        var resultado = percentual.ParaDecimal();

        // Assert
        resultado.Should().Be(0.30m);
    }

    [Fact]
    public void ParaDecimal_Deve_Retornar_0_Para_Percentual_0()
    {
        // Arrange
        var percentual = new Percentual(0);

        // Act & Assert
        percentual.ParaDecimal().Should().Be(0m);
    }

    [Fact]
    public void ParaDecimal_Deve_Retornar_1_Para_Percentual_100()
    {
        // Arrange
        var percentual = new Percentual(100);

        // Act & Assert
        percentual.ParaDecimal().Should().Be(1m);
    }

    [Fact]
    public void CalcularValor_DeveAplicarPercentualCorretamente()
    {
        // Arrange
        var percentual = new Percentual(20);
        var total = 1000m;

        // Act
        var resultado = percentual.CalcularValor(total);

        // Assert
        resultado.Should().Be(200m);
    }

    [Theory]
    [InlineData(10, 500, 50)]
    [InlineData(50, 1000, 500)]
    [InlineData(100, 100, 100)]
    public void CalcularValor_DeveComputarCorretamente(decimal percentual, decimal total, decimal esperado)
    {
        // Arrange
        var perc = new Percentual(percentual);

        // Act
        var resultado = perc.CalcularValor(total);

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void ToString_DeveRetornarFormatacao()
    {
        // Arrange
        var percentual = new Percentual(30);

        // Act
        var resultado = percentual.ToString();

        // Assert
        resultado.Should().Be("30.00%");
    }

    [Fact]
    public void Deve_SerIgual_QuandoValoresSaoIguais()
    {
        // Arrange
        var perc1 = new Percentual(25);
        var perc2 = new Percentual(25);

        // Act & Assert
        perc1.Should().Be(perc2);
        (perc1 == perc2).Should().BeTrue();
    }

    [Fact]
    public void Deve_SerDiferente_QuandoValoresSaoDiferentes()
    {
        // Arrange
        var perc1 = new Percentual(25);
        var perc2 = new Percentual(50);

        // Act & Assert
        perc1.Should().NotBe(perc2);
        (perc1 != perc2).Should().BeTrue();
    }

    [Fact]
    public void CompareTo_DeveRetornarNegativo_QuandoMenor()
    {
        // Arrange
        var perc1 = new Percentual(25);
        var perc2 = new Percentual(50);

        // Act & Assert
        perc1.CompareTo(perc2).Should().BeLessThan(0);
    }

    [Fact]
    public void CompareTo_DeveRetornarZero_QuandoIgual()
    {
        // Arrange
        var perc1 = new Percentual(25);
        var perc2 = new Percentual(25);

        // Act & Assert
        perc1.CompareTo(perc2).Should().Be(0);
    }

    [Fact]
    public void Operador_Menor_DeveSerValido()
    {
        // Arrange
        var perc1 = new Percentual(25);
        var perc2 = new Percentual(50);

        // Act & Assert
        (perc1 < perc2).Should().BeTrue();
    }

    [Fact]
    public void Operador_Maior_DeveSerValido()
    {
        // Arrange
        var perc1 = new Percentual(50);
        var perc2 = new Percentual(25);

        // Act & Assert
        (perc1 > perc2).Should().BeTrue();
    }
}
