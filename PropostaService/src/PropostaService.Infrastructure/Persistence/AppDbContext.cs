using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Proposta> Propostas => Set<Proposta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proposta>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.NomeProponente).IsRequired().HasMaxLength(200);
            e.Property(p => p.Cpf).IsRequired().HasMaxLength(14);
            e.Property(p => p.ValorCoberto).HasColumnType("decimal(18,2)");
            e.Property(p => p.Status).HasConversion<string>();
        });
    }
}
