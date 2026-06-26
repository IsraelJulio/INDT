namespace ContratacaoService.Infrastructure.Messaging;

public class PropostaStatusCache
{
    public Guid PropostaId { get; set; }
    public string Status { get; set; } = string.Empty;

    public DateTime AtualizadoEm { get; set; }
}
