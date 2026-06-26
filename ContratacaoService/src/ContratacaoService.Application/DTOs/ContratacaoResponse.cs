using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Application.DTOs;

public record ContratacaoResponse(
    Guid Id,
    Guid PropostaId,
    DateTime DataContratacao
)
{
    public static ContratacaoResponse FromContratacao(Contratacao c) =>
        new(c.Id, c.PropostaId, c.DataContratacao);
}
