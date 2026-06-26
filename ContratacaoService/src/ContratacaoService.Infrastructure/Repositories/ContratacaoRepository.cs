using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Ports;
using ContratacaoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Repositories;

public class ContratacaoRepository(AppDbContext context) : IContratacaoRepository
{
    public async Task<Contratacao> CriarAsync(Contratacao contratacao)
    {
        context.Contratacoes.Add(contratacao);
        await context.SaveChangesAsync();
        return contratacao;
    }

    public async Task<Contratacao?> ObterPorIdAsync(Guid id) =>
        await context.Contratacoes.FindAsync(id);

    public async Task<Contratacao?> ObterPorPropostaIdAsync(Guid propostaId) =>
        await context.Contratacoes.FirstOrDefaultAsync(c => c.PropostaId == propostaId);
}
