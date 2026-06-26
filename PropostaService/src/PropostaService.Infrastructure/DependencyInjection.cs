using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Ports;
using PropostaService.Infrastructure.Persistence;
using PropostaService.Infrastructure.Repositories;

namespace PropostaService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPropostaRepository, PropostaRepository>();

        services.AddScoped<CriarPropostaUseCase>();
        services.AddScoped<ListarPropostasUseCase>();
        services.AddScoped<ObterPropostaUseCase>();
        services.AddScoped<AtualizarStatusUseCase>();

        return services;
    }
}
