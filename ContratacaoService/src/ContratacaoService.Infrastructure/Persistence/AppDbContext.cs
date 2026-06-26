using ContratacaoService.Domain.Entities;
using ContratacaoService.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contratacao> Contratacoes => Set<Contratacao>();
    public DbSet<PropostaStatusCache> PropostaStatusCache => Set<PropostaStatusCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contratacao>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.PropostaId).IsUnique();
        });

        modelBuilder.Entity<PropostaStatusCache>(e =>
        {
            e.HasKey(c => c.PropostaId);
            e.Property(c => c.Status).HasMaxLength(50).IsRequired();
        });
    }
}
