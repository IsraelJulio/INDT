using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Ports;
using PropostaService.Infrastructure.Persistence;

namespace PropostaService.Infrastructure.Repositories;

public class PropostaRepository(AppDbContext context) : IPropostaRepository
{
    public async Task<Proposta> CriarAsync(Proposta proposta)
    {
        context.Propostas.Add(proposta);
        await context.SaveChangesAsync();
        return proposta;
    }

    public async Task<IEnumerable<Proposta>> ListarAsync() =>
        await context.Propostas.ToListAsync();

    public async Task<Proposta?> ObterPorIdAsync(Guid id) =>
        await context.Propostas.FindAsync(id);

    public async Task AtualizarAsync(Proposta proposta)
    {
        context.Propostas.Update(proposta);
        await context.SaveChangesAsync();
    }
}
