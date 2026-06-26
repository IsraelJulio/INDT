using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Ports;

public interface IContratacaoRepository
{
    Task<Contratacao> CriarAsync(Contratacao contratacao);
    Task<Contratacao?> ObterPorIdAsync(Guid id);
    Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId);
}
