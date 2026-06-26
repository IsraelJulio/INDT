using PropostaService.Domain.Entities;

namespace PropostaService.Application.DTOs;

public record PropostaResponse(
    Guid Id,
    string NomeProponente,
    string Cpf,
    decimal ValorCoberto,
    string Status,
    DateTime CriadoEm
)
{
    public static PropostaResponse FromProposta(Proposta p) =>
        new(p.Id, p.NomeProponente, p.Cpf, p.ValorCoberto, p.Status.ToString(), p.CriadoEm);
}
