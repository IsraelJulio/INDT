using PropostaService.Application.DTOs;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

public class ListarPropostasUseCase(IPropostaRepository repository)
{
    public async Task<IEnumerable<PropostaResponse>> ExecutarAsync()
    {
        var propostas = await repository.ListarAsync();
        return propostas.Select(PropostaResponse.FromProposta);
    }
}
