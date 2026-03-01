using CompraProgramada.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CompraProgramada.Tests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Deve_CriarDomainException_ComMensagem()
    {
        // Arrange & Act
        var exception = new DomainException("Erro teste");

        // Assert
        exception.Message.Should().Be("Erro teste");
        exception.CodigoErro.Should().Be("ERRO_DOMINIO");
    }

    [Fact]
    public void Deve_CriarDomainException_ComCodigoCustomizado()
    {
        // Arrange & Act
        var exception = new DomainException("Erro teste", "CODIGO_CUSTOM");

        // Assert
        exception.Message.Should().Be("Erro teste");
        exception.CodigoErro.Should().Be("CODIGO_CUSTOM");
    }

    [Fact]
    public void ClienteCpfDuplicadoException_Deve_Armazenar_CPF()
    {
        // Arrange & Act
        var exception = new ClienteCpfDuplicadoException("123.456.789-09");

        // Assert
        exception.CPF.Should().Be("123.456.789-09");
        exception.CodigoErro.Should().Be("CLIENTE_CPF_DUPLICADO");
        exception.Message.Should().Contain("123.456.789-09");
    }

    [Fact]
    public void ValorMensalInvalidoException_Deve_Armazenar_Valores()
    {
        // Arrange & Act
        var exception = new ValorMensalInvalidoException(50m, 100m);

        // Assert
        exception.ValorInformado.Should().Be(50m);
        exception.ValorMinimo.Should().Be(100m);
        exception.CodigoErro.Should().Be("VALOR_MENSAL_INVALIDO");
        exception.Message.Should().Contain("50");
        exception.Message.Should().Contain("100");
    }

    [Fact]
    public void PercentuaisInvalidosException_Deve_Armazenar_Soma()
    {
        // Arrange & Act
        var exception = new PercentuaisInvalidosException(99.5m);

        // Assert
        exception.SomaPercentuais.Should().Be(99.5m);
        exception.CodigoErro.Should().Be("PERCENTUAIS_INVALIDOS");
        exception.Message.Should().Contain("99.5");
    }

    [Fact]
    public void QuantidadeAtivosInvalidaException_Deve_Armazenar_Quantidades()
    {
        // Arrange & Act
        var exception = new QuantidadeAtivosInvalidaException(3, 5);

        // Assert
        exception.QuantidadeInformada.Should().Be(3);
        exception.QuantidadeEsperada.Should().Be(5);
        exception.CodigoErro.Should().Be("QUANTIDADE_ATIVOS_INVALIDA");
        exception.Message.Should().Contain("3");
        exception.Message.Should().Contain("5");
    }

    [Fact]
    public void ClienteNaoEncontradoException_Deve_Armazenar_Id()
    {
        // Arrange & Act
        var exception = new ClienteNaoEncontradoException(999);

        // Assert
        exception.ClienteId.Should().Be(999);
        exception.CodigoErro.Should().Be("CLIENTE_NAO_ENCONTRADO");
        exception.Message.Should().Contain("999");
    }

    [Fact]
    public void ClienteJaInativoException_Deve_Armazenar_Id()
    {
        // Arrange & Act
        var exception = new ClienteJaInativoException(888);

        // Assert
        exception.ClienteId.Should().Be(888);
        exception.CodigoErro.Should().Be("CLIENTE_JA_INATIVO");
        exception.Message.Should().Contain("888");
    }

    [Fact]
    public void CestaNaoEncontradaException_Deve_Ser_Criada()
    {
        // Arrange & Act
        var exception = new CestaNaoEncontradaException();

        // Assert
        exception.CodigoErro.Should().Be("CESTA_NAO_ENCONTRADA");
        exception.Message.Should().Contain("cesta");
    }

    [Fact]
    public void CotacaoNaoEncontradaException_Deve_Armazenar_Ticker_E_Data()
    {
        // Arrange
        var data = new DateTime(2026, 3, 1);

        // Act
        var exception = new CotacaoNaoEncontradaException("PETR4", data);

        // Assert
        exception.Ticker.Should().Be("PETR4");
        exception.Data.Should().Be(data);
        exception.CodigoErro.Should().Be("COTACAO_NAO_ENCONTRADA");
        exception.Message.Should().Contain("PETR4");
        exception.Message.Should().Contain("2026-03-01");
    }

    [Fact]
    public void CompraJaExecutadaException_Deve_Armazenar_Data()
    {
        // Arrange
        var data = new DateTime(2026, 3, 1);

        // Act
        var exception = new CompraJaExecutadaException(data);

        // Assert
        exception.DataReferencia.Should().Be(data);
        exception.CodigoErro.Should().Be("COMPRA_JA_EXECUTADA");
        exception.Message.Should().Contain("2026-03-01");
    }

    [Fact]
    public void KafkaIndisponivelException_Deve_Armazenar_InnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Conexão Kafka falhou");

        // Act
        var exception = new KafkaIndisponivelException("Erro ao conectar", innerException);

        // Assert
        exception.CodigoErro.Should().Be("KAFKA_INDISPONIVEL");
        exception.InnerException.Should().Be(innerException);
        exception.Message.Should().Contain("Erro ao conectar");
    }

    [Fact]
    public void Todas_Excecoes_Devem_Ser_Derivadas_De_DomainException()
    {
        // Arrange & Act & Assert
        var exceptions = new Exception[]
        {
            new ClienteCpfDuplicadoException("123.456.789-09"),
            new ValorMensalInvalidoException(50m),
            new PercentuaisInvalidosException(99m),
            new QuantidadeAtivosInvalidaException(3),
            new ClienteNaoEncontradoException(1),
            new ClienteJaInativoException(1),
            new CestaNaoEncontradaException(),
            new CotacaoNaoEncontradaException("PETR4", DateTime.Now),
            new CompraJaExecutadaException(DateTime.Now),
            new KafkaIndisponivelException("erro", new Exception())
        };

        foreach (var ex in exceptions)
        {
            ex.Should().BeOfType<DomainException>();
        }
    }
}
