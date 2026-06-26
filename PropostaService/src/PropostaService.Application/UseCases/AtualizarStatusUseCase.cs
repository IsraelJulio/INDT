using PropostaService.Application.DTOs;
using PropostaService.Application.Events;
using PropostaService.Application.Ports;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

public class AtualizarStatusUseCase(IPropostaRepository repository, IEventPublisher eventPublisher)
{
    private const string TopicPropostaStatus = "proposta-status-atualizada";

    public async Task<PropostaResponse?> ExecutarAsync(Guid id, AtualizarStatusRequest request)
    {
        var proposta = await repository.ObterPorIdAsync(id);
        if (proposta is null) return null;

        if (!Enum.TryParse<StatusProposta>(request.Status, ignoreCase: true, out var novoStatus))
            throw new ArgumentException($"Status inválido: {request.Status}");

        proposta.AtualizarStatus(novoStatus);
        await repository.AtualizarAsync(proposta);




        await eventPublisher.PublishAsync(TopicPropostaStatus, new PropostaStatusAtualizadaEvent(
            proposta.Id,
            novoStatus.ToString(),
            DateTime.UtcNow));




        return PropostaResponse.FromProposta(proposta);
    }
}
