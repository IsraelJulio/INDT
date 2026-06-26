using ContratacaoService.Application.UseCases;
using ContratacaoService.Domain.Ports;
using ContratacaoService.Infrastructure.HttpClients;
using ContratacaoService.Infrastructure.Persistence;
using ContratacaoService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ContratacaoService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IContratacaoRepository, ContratacaoRepository>();

        services.AddHttpClient<IPropostaClient, PropostaHttpClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["PropostaService:BaseUrl"]!);
        });

        services.AddScoped<ContratarPropostaUseCase>();

        return services;
    }
}
