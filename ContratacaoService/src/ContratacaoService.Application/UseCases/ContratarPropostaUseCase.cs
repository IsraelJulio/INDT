using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Application.UseCases;

public class ContratarPropostaUseCase(
    IContratacaoRepository repository,
    IPropostaClient propostaClient)
{
    public async Task<ContratacaoResponse> ExecutarAsync(ContratarRequest request)
    {
        var jaContratada = await repository.ObterPorPropostaIdAsync(request.PropostaId);
        if (jaContratada is not null)
            throw new InvalidOperationException("Proposta já foi contratada.");

        var status = await propostaClient.ObterStatusPropostaAsync(request.PropostaId);

        if (status is null)
            throw new KeyNotFoundException($"Proposta {request.PropostaId} não encontrada.");

        if (status != "Aprovada")
            throw new InvalidOperationException($"Proposta não pode ser contratada. Status atual: {status}");

        var contratacao = Contratacao.Criar(request.PropostaId);
        await repository.CriarAsync(contratacao);

        return ContratacaoResponse.FromContratacao(contratacao);
    }
}
