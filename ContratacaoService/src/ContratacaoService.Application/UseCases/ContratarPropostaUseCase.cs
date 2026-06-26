using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;

namespace ContratacaoService.Application.UseCases;

public class ContratarPropostaUseCase(
    IContratacaoRepository repository,
    IPropostaStatusCache propostaStatusCache)
{
    public async Task<ContratacaoResponse> ExecutarAsync(ContratarRequest request)
    {
        var jaContratada = await repository.ObterPorPropostaIdAsync(request.PropostaId);

        if (jaContratada is not null)
            throw new InvalidOperationException("Proposta já foi contratada.");



        var status = await propostaStatusCache.ObterStatusAsync(request.PropostaId)
            ?? throw new KeyNotFoundException($"Proposta {request.PropostaId} não encontrada ou ainda não processada.");
        if (status != "Aprovada")
            throw new InvalidOperationException($"Proposta não pode ser contratada. Status atual: {status}");


        var contratacao = new Contratacao(request.PropostaId);
        await repository.CriarAsync(contratacao);

        return ContratacaoResponse.FromContratacao(contratacao);
    }
}
