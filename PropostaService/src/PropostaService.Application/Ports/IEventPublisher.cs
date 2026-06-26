namespace PropostaService.Application.Ports;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, T evento) where T : class;
}
