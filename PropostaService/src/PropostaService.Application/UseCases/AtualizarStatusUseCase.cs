using PropostaService.Application.DTOs;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

public class AtualizarStatusUseCase(IPropostaRepository repository)
{
    public async Task<PropostaResponse?> ExecutarAsync(Guid id, AtualizarStatusRequest request)
    {
        var proposta = await repository.ObterPorIdAsync(id);
        if (proposta is null) return null;

        if (!Enum.TryParse<StatusProposta>(request.Status, ignoreCase: true, out var novoStatus))
            throw new ArgumentException($"Status inválido: {request.Status}");

        proposta.AtualizarStatus(novoStatus);
        await repository.AtualizarAsync(proposta);

        return PropostaResponse.FromProposta(proposta);
    }
}
