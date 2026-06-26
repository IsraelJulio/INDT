using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    public Guid Id { get; private set; }
    public string NomeProponente { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public decimal ValorCoberto { get; private set; }
    public StatusProposta Status { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Proposta() { }

    public static Proposta Criar(string nomeProponente, string cpf, decimal valorCoberto)
    {
        return new Proposta
        {
            Id = Guid.NewGuid(),
            NomeProponente = nomeProponente,
            Cpf = cpf,
            ValorCoberto = valorCoberto,
            Status = StatusProposta.EmAnalise,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void AtualizarStatus(StatusProposta novoStatus)
    {
        Status = novoStatus;
    }
}
