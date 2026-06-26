using Confluent.Kafka;
using ContratacaoService.Domain.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ContratacaoService.Infrastructure.Messaging;

public class KafkaPropostaStatusConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<KafkaPropostaStatusConsumer> _logger;
    private readonly ConsumerConfig _config;
    private const string Topic = "proposta-status-atualizada";

    public KafkaPropostaStatusConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<KafkaPropostaStatusConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "contratacao-service",
            AutoOffsetReset = AutoOffsetReset.Earliest

        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();


        using var consumer = new ConsumerBuilder<string, string>(_config).Build();
        consumer.Subscribe(Topic);
        _logger.LogInformation("Aguardando eventos do topic {Topic}...", Topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);
                if (result?.Message is null) continue;


                var evento = JsonSerializer.Deserialize<PropostaStatusAtualizadaEvent>(result.Message.Value);
                if (evento is null) continue;


                using var scope = _scopeFactory.CreateScope();
                var cache = scope.ServiceProvider.GetRequiredService<IPropostaStatusCache>();
                await cache.SalvarAsync(evento.PropostaId, evento.NovoStatus);

                _logger.LogInformation("Status da proposta {PropostaId} atualizado para '{Status}'",
                                        evento.PropostaId, evento.NovoStatus);
            }
            catch (OperationCanceledException)
            { break; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do Kafka");
            }
        }

        consumer.Close();
    }
}
