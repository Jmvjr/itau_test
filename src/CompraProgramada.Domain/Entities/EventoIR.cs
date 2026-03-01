using CompraProgramada.Domain.Enums;

namespace CompraProgramada.Domain.Entities;

/// <summary>
/// Entidade EventoIR — Evento de imposto de renda para publicação no Kafka
/// RN-053 a RN-062
/// </summary>
public class EventoIR
{
    public long Id { get; private set; }
    public long ClienteId { get; private set; }
    public TipoEventoIR Tipo { get; private set; }
    public decimal ValorBase { get; private set; } // Valor da operação ou lucro
    public decimal ValorIR { get; private set; } // IR calculado
    public bool PublicadoKafka { get; private set; }
    public DateTime DataEvento { get; private set; }

    protected EventoIR() { }

    /// <summary>
    /// Criar evento de IR dedo-duro (0,005%) - RN-053, RN-054
    /// </summary>
    public static EventoIR CriarDedoDuro(long clienteId, decimal valorOperacao)
    {
        if (valorOperacao <= 0)
            throw new ArgumentException("Valor da operação deve ser maior que zero", nameof(valorOperacao));

        const decimal aliquotaDedoDuro = 0.00005m; // 0,005%
        var valorIR = Math.Round(valorOperacao * aliquotaDedoDuro, 2);

        return new EventoIR
        {
            ClienteId = clienteId,
            Tipo = TipoEventoIR.DedoDuro,
            ValorBase = valorOperacao,
            ValorIR = valorIR,
            PublicadoKafka = false,
            DataEvento = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Criar evento de IR sobre venda (20% se vendas > R$ 20mil) - RN-059
    /// </summary>
    public static EventoIR CriarIRVenda(long clienteId, decimal lucroLiquido)
    {
        const decimal aliquotaIR = 0.20m; // 20%
        decimal valorIR = 0m;

        // Se lucro é positivo, calcula 20%
        if (lucroLiquido > 0)
            valorIR = Math.Round(lucroLiquido * aliquotaIR, 2);

        return new EventoIR
        {
            ClienteId = clienteId,
            Tipo = TipoEventoIR.IRVenda,
            ValorBase = lucroLiquido,
            ValorIR = valorIR,
            PublicadoKafka = false,
            DataEvento = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Marcar como publicado no Kafka
    /// </summary>
    public void MarcarPublicado()
    {
        PublicadoKafka = true;
    }

    public override string ToString() => 
        $"{Tipo}: R$ {ValorIR:F2} sobre R$ {ValorBase:F2}";
}
