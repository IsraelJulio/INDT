using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PropostaService.Application.Ports;
using System.Text.Json;

namespace PropostaService.Infrastructure.Messaging;

public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;





    public KafkaEventPublisher(IConfiguration configuration, ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;
        var bootstrapServers = configuration["Kafka:BootstrapServers"]!;

        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T evento) where T : class
    {
        var payload = JsonSerializer.Serialize(evento);
        var message = new Message<string, string> { Value = payload };


        await _producer.ProduceAsync(topic, message);

        _logger.LogInformation("Evento publicado no topic {Topic}", topic);
    }

    public void Dispose() => _producer.Dispose();
}
