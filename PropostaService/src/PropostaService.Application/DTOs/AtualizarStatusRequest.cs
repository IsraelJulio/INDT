using System.ComponentModel.DataAnnotations;

namespace PropostaService.Application.DTOs;

public record AtualizarStatusRequest([Required] string Status);
