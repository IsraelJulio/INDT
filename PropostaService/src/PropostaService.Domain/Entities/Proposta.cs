using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    private Proposta() { }

    public Proposta(string nomeProponente, string cpf, decimal valorCoberto)
    {
        Id = Guid.NewGuid();
        NomeProponente = nomeProponente;
        Cpf = cpf;
        ValorCoberto = valorCoberto;
        Status = StatusProposta.EmAnalise;
        CriadoEm = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    public string NomeProponente { get; init; } = string.Empty;
    public string Cpf { get; init; } = string.Empty;
    public decimal ValorCoberto { get; init; }
    public StatusProposta Status { get; private set; }
    public DateTime CriadoEm { get; init; }

    public void AtualizarStatus(StatusProposta novoStatus) => Status = novoStatus;
}
