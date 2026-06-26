namespace ContratacaoService.Domain.Ports;

public interface IPropostaClient
{
    Task<string?> ObterStatusPropostaAsync(Guid propostaId);
}
