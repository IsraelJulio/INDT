using PropostaService.Domain.Entities;

namespace PropostaService.Domain.Ports;

public interface IPropostaRepository
{
    Task<Proposta> CriarAsync(Proposta proposta);
    Task<IEnumerable<Proposta>> ListarAsync();
    Task<Proposta?> ObterPorIdAsync(Guid id);
    Task AtualizarAsync(Proposta proposta);
}
