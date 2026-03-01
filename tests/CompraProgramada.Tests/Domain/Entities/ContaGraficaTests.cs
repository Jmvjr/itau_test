using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class ContaGraficaTests
{
    [Fact]
    public void Deve_Criar_Conta_Master()
    {
        // Arrange & Act
        var conta = ContaGrafica.CriarContaMaster("MST-000001");

        // Assert
        conta.Tipo.Should().Be(TipoConta.Master);
        conta.NumeroConta.Should().Be("MST-000001");
    }

    [Fact]
    public void Deve_Criar_Conta_Filhote()
    {
        // Arrange & Act
        var conta = ContaGrafica.CriarContaFilhote(123, "FLH-000001");

        // Assert
        conta.Tipo.Should().Be(TipoConta.Filhote);
        conta.NumeroConta.Should().Be("FLH-000001");
        conta.ClienteId.Should().Be(123);
    }

    [Fact]
    public void Deve_Gerar_Numeros_Diferentes_Master()
    {
        // Arrange & Act
        var conta1 = ContaGrafica.CriarContaMaster("MST-000001");
        var conta2 = ContaGrafica.CriarContaMaster("MST-000002");

        // Assert - números devem ser diferentes
        conta1.NumeroConta.Should().NotBe(conta2.NumeroConta);
    }

    [Fact]
    public void Deve_Gerar_Numeros_Diferentes_Filhote()
    {
        // Arrange & Act
        var conta1 = ContaGrafica.CriarContaFilhote(1, "FLH-000001");
        var conta2 = ContaGrafica.CriarContaFilhote(2, "FLH-000002");

        // Assert
        conta1.NumeroConta.Should().NotBe(conta2.NumeroConta);
    }

    [Fact]
    public void Deve_Registrar_Data_Criacao()
    {
        // Arrange & Act
        var conta = ContaGrafica.CriarContaMaster("MST-000001");

        // Assert
        conta.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Master_Nao_Deve_Ter_ClienteId()
    {
        // Arrange & Act
        var conta = ContaGrafica.CriarContaMaster("MST-000001");

        // Assert
        conta.ClienteId.Should().BeNull();
    }

    [Fact]
    public void Filhote_Deve_Ter_ClienteId()
    {
        // Arrange & Act
        var conta = ContaGrafica.CriarContaFilhote(999, "FLH-000001");

        // Assert
        conta.ClienteId.Should().Be(999);
    }
}
