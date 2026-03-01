using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Entities;

public class EventoIRTests
{
    [Fact]
    public void Deve_Criar_Dedo_Duro_Com_0_005_Porcento()
    {
        // Arrange & Act
        var evento = EventoIR.CriarDedoDuro(clienteId: 123, valorOperacao: 1000m);

        // Assert
        evento.ClienteId.Should().Be(123);
        evento.Tipo.Should().Be(TipoEventoIR.DedoDuro);
        evento.ValorBase.Should().Be(1000m);
        evento.ValorIR.Should().Be(0.50m); // 0.005% de 1000
    }

    [Fact]
    public void Deve_Criar_IR_Venda_Com_20_Porcento_Se_Lucro()
    {
        // Arrange & Act
        var evento = EventoIR.CriarIRVenda(clienteId: 456, lucroLiquido: 500m);

        // Assert
        evento.ClienteId.Should().Be(456);
        evento.Tipo.Should().Be(TipoEventoIR.IRVenda);
        evento.ValorBase.Should().Be(500m);
        evento.ValorIR.Should().Be(100m); // 20% de 500
    }

    [Fact]
    public void Deve_Retornar_Zero_IR_Se_Lucro_Negativo()
    {
        // Arrange & Act
        var evento = EventoIR.CriarIRVenda(clienteId: 456, lucroLiquido: -300m);

        // Assert
        evento.ValorIR.Should().Be(0m);
    }

    [Fact]
    public void Deve_Retornar_Zero_IR_Se_Lucro_Zero()
    {
        // Arrange & Act
        var evento = EventoIR.CriarIRVenda(clienteId: 456, lucroLiquido: 0m);

        // Assert
        evento.ValorIR.Should().Be(0m);
    }

    [Fact]
    public void Deve_Iniciar_Nao_Publicado()
    {
        // Arrange & Act
        var evento = EventoIR.CriarDedoDuro(123, 1000m);

        // Assert
        evento.PublicadoKafka.Should().BeFalse();
    }

    [Fact]
    public void Deve_Marcar_Como_Publicado()
    {
        // Arrange
        var evento = EventoIR.CriarDedoDuro(123, 1000m);
        evento.PublicadoKafka.Should().BeFalse();

        // Act
        evento.MarcarPublicado();

        // Assert
        evento.PublicadoKafka.Should().BeTrue();
    }

    [Fact]
    public void Deve_Calcular_Dedo_Duro_Com_Precisao()
    {
        // Arrange & Act
        var evento = EventoIR.CriarDedoDuro(clienteId: 1, valorOperacao: 12345.67m);

        // Assert - 0.005% de 12345.67 = 0.6173335
        evento.ValorIR.Should().BeApproximately(0.62m, 0.01m);
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Operacao_Negativo()
    {
        // Arrange & Act & Assert
        var act = () => EventoIR.CriarDedoDuro(clienteId: 1, valorOperacao: -100m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deve_Rejeitar_Valor_Operacao_Zero()
    {
        // Arrange & Act & Assert
        var act = () => EventoIR.CriarDedoDuro(clienteId: 1, valorOperacao: 0m);
        act.Should().Throw<ArgumentException>();
    }
}
