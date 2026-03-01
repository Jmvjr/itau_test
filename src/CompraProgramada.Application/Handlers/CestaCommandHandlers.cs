using MediatR;
using CompraProgramada.Application.Commands;
using CompraProgramada.Domain.Entities;
using CompraProgramada.Domain.Interfaces;
using CompraProgramada.Application.Services;
using CompraProgramada.Application.EventHandlers;

namespace CompraProgramada.Application.Handlers;

/// <summary>
/// Handler para criar uma nova CestaRecomendacao
/// RN-014 a RN-016
/// </summary>
public class CriarCestaCommandHandler : IRequestHandler<CriarCestaCommand, long>
{
    private readonly ICestaRepository _cestaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CriarCestaCommandHandler(ICestaRepository cestaRepository, IUnitOfWork unitOfWork)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<long> Handle(CriarCestaCommand request, CancellationToken cancellationToken)
    {
        // Criar nova cesta (validações de domínio ocorrem no construtor)
        var cesta = new CestaRecomendacao(request.Nome, request.Itens);

        // Persistir
        await _cestaRepository.AdicionarAsync(cesta);
        await _unitOfWork.SaveChangesAsync();

        return cesta.Id;
    }
}

/// <summary>
/// Handler para ativar uma CestaRecomendacao
/// RN-019: Ativação dispara rebalanceamento de todos os clientes
/// </summary>
public class AtivarCestaCommandHandler : IRequestHandler<AtivarCestaCommand, bool>
{
    private readonly ICestaRepository _cestaRepository;
    private readonly IRebalanceamentoService _rebalanceamentoService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublisher _publisher;

    public AtivarCestaCommandHandler(
        ICestaRepository cestaRepository,
        IRebalanceamentoService rebalanceamentoService,
        IUnitOfWork unitOfWork,
        IPublisher publisher)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
        _rebalanceamentoService = rebalanceamentoService ?? throw new ArgumentNullException(nameof(rebalanceamentoService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task<bool> Handle(AtivarCestaCommand request, CancellationToken cancellationToken)
    {
        // Obter cesta a ser ativada
        var novaCesta = await _cestaRepository.ObterPorIdAsync(request.CestaId);
        if (novaCesta == null)
            throw new InvalidOperationException($"Cesta {request.CestaId} não encontrada");

        // Obter cesta ativa anterior
        var cestaAnterior = await _cestaRepository.ObterAtivaAsync();

        // Desativar cesta anterior se houver
        if (cestaAnterior != null)
        {
            cestaAnterior.Desativar();
            _cestaRepository.Atualizar(cestaAnterior);
        }

        // Marcar nova cesta como ativa
        // Nota: A cesta é criada com Ativa=true por padrão no construtor
        // Este método é para fazer upload de uma cesta previamente criada
        
        await _unitOfWork.SaveChangesAsync();

        // RN-019: Disparar rebalanceamento de todos os clientes
        if (cestaAnterior != null)
        {
            await _rebalanceamentoService.ProcessarRebalanceamentoAsync(cestaAnterior, novaCesta);
        }

        // Publicar evento de cesta ativada
        var itens = novaCesta.Itens
            .Select(i => (i.Ticker.Valor, i.Percentual.Valor))
            .ToList();

        var eventoAtivacao = new CestaAtivadaEvent(
            novaCesta.Id,
            novaCesta.Nome,
            cestaAnterior?.Id,
            cestaAnterior?.Nome,
            itens);

        await _publisher.Publish(eventoAtivacao, cancellationToken);

        return true;
    }
}

/// <summary>
/// Handler para desativar uma CestaRecomendacao
/// RN-017
/// </summary>
public class DesativarCestaCommandHandler : IRequestHandler<DesativarCestaCommand, bool>
{
    private readonly ICestaRepository _cestaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DesativarCestaCommandHandler(ICestaRepository cestaRepository, IUnitOfWork unitOfWork)
    {
        _cestaRepository = cestaRepository ?? throw new ArgumentNullException(nameof(cestaRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(DesativarCestaCommand request, CancellationToken cancellationToken)
    {
        // Obter cesta
        var cesta = await _cestaRepository.ObterPorIdAsync(request.CestaId);
        if (cesta == null)
            throw new InvalidOperationException($"Cesta {request.CestaId} não encontrada");

        // Desativar (RN-017)
        cesta.Desativar();

        // Persistir
        _cestaRepository.Atualizar(cesta);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
