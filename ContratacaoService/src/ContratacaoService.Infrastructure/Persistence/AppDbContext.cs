using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contratacao>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.PropostaId).IsUnique();
        });
    }
}
