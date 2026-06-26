namespace ContratacaoService.Infrastructure.Messaging;

public record PropostaStatusAtualizadaEvent(
    Guid PropostaId,
    string NovoStatus,
    DateTime OcorreuEm);
