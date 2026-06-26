namespace ContratacaoService.Domain.Entities;

public class Contratacao
{
    private Contratacao() { }

    public Contratacao(Guid propostaId)
    {
        Id = Guid.NewGuid();
        PropostaId = propostaId;
        DataContratacao = DateTime.UtcNow;

    }

    public Guid Id { get; init; }
    public Guid PropostaId { get; init; }
    public DateTime DataContratacao { get; init; }
}
