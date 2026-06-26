using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases;

namespace PropostaService.API.Controllers;

[ApiController]
[Route("propostas")]
public class PropostaController(
    CriarPropostaUseCase criarUseCase,
    ListarPropostasUseCase listarUseCase,
    ObterPropostaUseCase obterUseCase,
    AtualizarStatusUseCase atualizarStatusUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPropostaRequest request)
    {
        var response = await criarUseCase.ExecutarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var propostas = await listarUseCase.ExecutarAsync();
        return Ok(propostas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var proposta = await obterUseCase.ExecutarAsync(id);
        return proposta is null ? NotFound() : Ok(proposta);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AtualizarStatusRequest request)
    {
        try
        {
            var resultado = await atualizarStatusUseCase.ExecutarAsync(id, request);
            return resultado is null ? NotFound() : Ok(resultado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
