using System.ComponentModel.DataAnnotations;

namespace ContratacaoService.Application.DTOs;

public record ContratarRequest([Required] Guid PropostaId);
