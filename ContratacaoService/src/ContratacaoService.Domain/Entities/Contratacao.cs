namespace ContratacaoService.Domain.Entities;

public class Contratacao
{
    public Guid Id { get; private set; }
    public Guid PropostaId { get; private set; }
    public DateTime DataContratacao { get; private set; }

    private Contratacao() { }

    public static Contratacao Criar(Guid propostaId)
    {
        return new Contratacao
        {
            Id = Guid.NewGuid(),
            PropostaId = propostaId,
            DataContratacao = DateTime.UtcNow
        };
    }
}
