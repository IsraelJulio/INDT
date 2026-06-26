using ContratacaoService.Domain.Ports;
using ContratacaoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Messaging;

public class PropostaStatusCacheRepository(AppDbContext context) : IPropostaStatusCache
{
    public async Task SalvarAsync(Guid propostaId, string status)
    {
        var existente = await context.PropostaStatusCache.FindAsync(propostaId);
        if (existente is null)
        {
            context.PropostaStatusCache.Add(new PropostaStatusCache
            {
                PropostaId = propostaId,
                Status = status,
                AtualizadoEm = DateTime.UtcNow
            });
        }
        else
        {
            existente.Status = status;
            existente.AtualizadoEm = DateTime.UtcNow;
        }


        await context.SaveChangesAsync();
    }

    public async Task<string?> ObterStatusAsync(Guid propostaId)
    {
        var cache = await context.PropostaStatusCache.FindAsync(propostaId);

        return cache?.Status;
    }
}
