using MediatR;

namespace CompraProgramada.Application.Commands;

/// <summary>
/// Comando para criar uma nova CestaRecomendacao
/// RN-014 a RN-016
/// </summary>
public class CriarCestaCommand : IRequest<long>
{
    public string Nome { get; set; }
    public List<(string ticker, decimal percentual)> Itens { get; set; }

    public CriarCestaCommand(string nome, List<(string, decimal)> itens)
    {
        Nome = nome;
        Itens = itens;
    }
}

/// <summary>
/// Comando para ativar uma CestaRecomendacao
/// RN-019: Ativação dispara rebalanceamento de todos os clientes
/// </summary>
public class AtivarCestaCommand : IRequest<bool>
{
    public long CestaId { get; set; }

    public AtivarCestaCommand(long cestaId)
    {
        CestaId = cestaId;
    }
}

/// <summary>
/// Comando para desativar uma CestaRecomendacao
/// RN-017
/// </summary>
public class DesativarCestaCommand : IRequest<bool>
{
    public long CestaId { get; set; }

    public DesativarCestaCommand(long cestaId)
    {
        CestaId = cestaId;
    }
}
