using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Exceptions;
using CompraProgramada.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Deve_CriarCliente_ComDadosValidos()
    {
        // Arrange & Act
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 500m);

        // Assert
        cliente.Nome.Should().Be("João Silva");
        cliente.CPF.Valor.Should().Be("12345678909");
        cliente.Email.Should().Be("joao@email.com");
        cliente.ValorMensal.Should().Be(500m);
        cliente.Ativo.Should().BeTrue();
        cliente.DataAdesao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Mensal_Menor_Que_Minimo()
    {
        // Arrange & Act & Assert
        var act = () => new Cliente("João Silva", "123.456.789-09", "joao@email.com", 50m);
        act.Should().Throw<ValorMensalInvalidoException>();
    }

    [Fact]
    public void Deve_Aceitar_Valor_Mensal_Minimo()
    {
        // Arrange & Act
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 100m);

        // Assert
        cliente.ValorMensal.Should().Be(100m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Rejeitar_Nome_Vazio(string nome)
    {
        // Arrange & Act & Assert
        var act = () => new Cliente(nome, "123.456.789-09", "joao@email.com", 500m);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_Rejeitar_Email_Vazio(string email)
    {
        // Arrange & Act & Assert
        var act = () => new Cliente("João Silva", "123.456.789-09", email, 500m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_CPF_Invalido()
    {
        // Arrange & Act & Assert
        var act = () => new Cliente("João Silva", "111.111.111-11", "joao@email.com", 500m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Calcular_Valor_Parcela_Como_Um_Terco()
    {
        // Arrange
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 300m);

        // Act
        var parcela = cliente.CalcularValorParcela();

        // Assert
        parcela.Should().Be(100m);
    }

    [Fact]
    public void Deve_Sair_Desativando_Cliente()
    {
        // Arrange
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 500m);
        cliente.Ativo.Should().BeTrue();

        // Act
        cliente.Sair();

        // Assert
        cliente.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Deve_Rejeitar_Sair_Cliente_Ja_Inativo()
    {
        // Arrange
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 500m);
        cliente.Sair();

        // Act & Assert
        var act = () => cliente.Sair();
        act.Should().Throw<ClienteJaInativoException>();
    }

    [Fact]
    public void Deve_Alterar_Valor_Mensal()
    {
        // Arrange
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 500m);

        // Act
        cliente.AlterarValorMensal(1000m);

        // Assert
        cliente.ValorMensal.Should().Be(1000m);
    }

    [Fact]
    public void Deve_Rejeitar_Alterar_Valor_Mensal_Invalido()
    {
        // Arrange
        var cliente = new Cliente("João Silva", "123.456.789-09", "joao@email.com", 500m);

        // Act & Assert
        var act = () => cliente.AlterarValorMensal(50m);
        act.Should().Throw<ValorMensalInvalidoException>();
    }
}
