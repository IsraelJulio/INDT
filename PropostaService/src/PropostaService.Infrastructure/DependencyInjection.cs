using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropostaService.Application.Ports;
using PropostaService.Application.UseCases;
using PropostaService.Domain.Ports;
using PropostaService.Infrastructure.Messaging;
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



        services.AddSingleton<IEventPublisher, KafkaEventPublisher>();


        services.AddScoped<CriarPropostaUseCase>();
        services.AddScoped<ListarPropostasUseCase>();
        services.AddScoped<ObterPropostaUseCase>();
        services.AddScoped<AtualizarStatusUseCase>();

        return services;
    }
}
