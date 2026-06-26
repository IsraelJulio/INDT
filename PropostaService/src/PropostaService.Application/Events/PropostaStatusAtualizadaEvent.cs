namespace PropostaService.Application.Events;

public record PropostaStatusAtualizadaEvent(
    Guid PropostaId,
    string NovoStatus,
    DateTime OcorreuEm);
