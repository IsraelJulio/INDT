namespace ContratacaoService.Domain.Ports;

public interface IPropostaStatusCache
{
    Task SalvarAsync(Guid propostaId, string status);
    Task<string?> ObterStatusAsync(Guid propostaId);
}
