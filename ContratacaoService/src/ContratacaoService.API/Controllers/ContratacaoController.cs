using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace ContratacaoService.API.Controllers;

[ApiController]
[Route("contratacoes")]
public class ContratacaoController(
    ContratarPropostaUseCase contratarUseCase,
    IContratacaoRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Contratar([FromBody] ContratarRequest request)
    {
        try
        {
            var resultado = await contratarUseCase.ExecutarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(new { erro = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var contratacao = await repository.ObterPorIdAsync(id);
        if (contratacao is null) return NotFound();
        return Ok(ContratacaoResponse.FromContratacao(contratacao));
    }
}
