using PropostaService.Application.DTOs;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports;

namespace PropostaService.Application.UseCases;

public class CriarPropostaUseCase(IPropostaRepository repository)
{
    public async Task<PropostaResponse> ExecutarAsync(CriarPropostaRequest request)
    {
        var proposta = Proposta.Criar(request.NomeProponente, request.Cpf, request.ValorCoberto);
        await repository.CriarAsync(proposta);
        return PropostaResponse.FromProposta(proposta);
    }
}
