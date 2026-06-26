using PropostaService.Application.DTOs;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

public class ObterPropostaUseCase(IPropostaRepository repository)
{
    public async Task<PropostaResponse?> ExecutarAsync(Guid id)
    {
        var proposta = await repository.ObterPorIdAsync(id);
        return proposta is null ? null : PropostaResponse.FromProposta(proposta);
    }
}
