using System.ComponentModel.DataAnnotations;

namespace PropostaService.Application.DTOs;

public record CriarPropostaRequest(
    [Required] string NomeProponente,
    [Required] string Cpf,
    [Range(0.01, double.MaxValue)] decimal ValorCoberto
);
