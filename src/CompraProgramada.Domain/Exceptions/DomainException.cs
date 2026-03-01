namespace CompraProgramada.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio
/// </summary>
public class DomainException : Exception
{
    public string CodigoErro { get; set; }

    public DomainException(string mensagem, string codigoErro = "ERRO_DOMINIO") 
        : base(mensagem)
    {
        CodigoErro = codigoErro;
    }

    public DomainException(string mensagem, Exception innerException, string codigoErro = "ERRO_DOMINIO")
        : base(mensagem, innerException)
    {
        CodigoErro = codigoErro;
    }
}

/// <summary>
/// Exceção lançada quando CPF já existe no sistema
/// </summary>
public class ClienteCpfDuplicadoException : DomainException
{
    public string CPF { get; }

    public ClienteCpfDuplicadoException(string cpf)
        : base($"CPF {cpf} já cadastrado no sistema.", "CLIENTE_CPF_DUPLICADO")
    {
        CPF = cpf;
    }
}

/// <summary>
/// Exceção lançada quando valor mensal é inválido
/// </summary>
public class ValorMensalInvalidoException : DomainException
{
    public decimal ValorInformado { get; }
    public decimal ValorMinimo { get; }

    public ValorMensalInvalidoException(decimal valorInformado, decimal valorMinimo = 100m)
        : base(
            $"Valor mensal deve ser no mínimo R$ {valorMinimo:F2}. Informado: R$ {valorInformado:F2}",
            "VALOR_MENSAL_INVALIDO")
    {
        ValorInformado = valorInformado;
        ValorMinimo = valorMinimo;
    }
}

/// <summary>
/// Exceção lançada quando percentuais não somam 100%
/// </summary>
public class PercentuaisInvalidosException : DomainException
{
    public decimal SomaPercentuais { get; }

    public PercentuaisInvalidosException(decimal somaPercentuais)
        : base(
            $"Soma dos percentuais deve ser exatamente 100%. Soma atual: {somaPercentuais:F2}%.",
            "PERCENTUAIS_INVALIDOS")
    {
        SomaPercentuais = somaPercentuais;
    }
}

/// <summary>
/// Exceção lançada quando quantidade de ativos é inválida
/// </summary>
public class QuantidadeAtivosInvalidaException : DomainException
{
    public int QuantidadeInformada { get; }
    public int QuantidadeEsperada { get; }

    public QuantidadeAtivosInvalidaException(int quantidadeInformada, int quantidadeEsperada = 5)
        : base(
            $"Cesta deve conter exatamente {quantidadeEsperada} ativos. Quantidade informada: {quantidadeInformada}.",
            "QUANTIDADE_ATIVOS_INVALIDA")
    {
        QuantidadeInformada = quantidadeInformada;
        QuantidadeEsperada = quantidadeEsperada;
    }
}

/// <summary>
/// Exceção lançada quando cliente não é encontrado
/// </summary>
public class ClienteNaoEncontradoException : DomainException
{
    public long ClienteId { get; }

    public ClienteNaoEncontradoException(long clienteId)
        : base($"Cliente com ID {clienteId} não encontrado.", "CLIENTE_NAO_ENCONTRADO")
    {
        ClienteId = clienteId;
    }
}

/// <summary>
/// Exceção lançada quando cliente está inativo
/// </summary>
public class ClienteJaInativoException : DomainException
{
    public long ClienteId { get; }

    public ClienteJaInativoException(long clienteId)
        : base($"Cliente com ID {clienteId} já está inativo.", "CLIENTE_JA_INATIVO")
    {
        ClienteId = clienteId;
    }
}

/// <summary>
/// Exceção lançada quando cesta não é encontrada
/// </summary>
public class CestaNaoEncontradaException : DomainException
{
    public CestaNaoEncontradaException()
        : base("Nenhuma cesta ativa encontrada.", "CESTA_NAO_ENCONTRADA")
    {
    }
}

/// <summary>
/// Exceção lançada quando cotação não é encontrada
/// </summary>
public class CotacaoNaoEncontradaException : DomainException
{
    public string Ticker { get; }
    public new DateTime Data { get; }

    public CotacaoNaoEncontradaException(string ticker, DateTime data)
        : base(
            $"Cotação não encontrada para {ticker} em {data:yyyy-MM-dd}.",
            "COTACAO_NAO_ENCONTRADA")
    {
        Ticker = ticker;
        Data = data;
    }
}

/// <summary>
/// Exceção lançada quando compra já foi executada no mesmo dia
/// </summary>
public class CompraJaExecutadaException : DomainException
{
    public DateTime DataReferencia { get; }

    public CompraJaExecutadaException(DateTime dataReferencia)
        : base(
            $"Compra já foi executada para {dataReferencia:yyyy-MM-dd}.",
            "COMPRA_JA_EXECUTADA")
    {
        DataReferencia = dataReferencia;
    }
}

/// <summary>
/// Exceção lançada quando há erro ao publicar no Kafka
/// </summary>
public class KafkaIndisponivelException : DomainException
{
    public KafkaIndisponivelException(string mensagem, Exception innerException)
        : base($"Erro ao publicar no Kafka: {mensagem}", innerException, "KAFKA_INDISPONIVEL")
    {
    }
}
