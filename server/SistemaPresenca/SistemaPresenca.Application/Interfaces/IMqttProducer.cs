namespace SistemaPresenca.Application.Interfaces;

public interface IMqttProducer
{
    Task PublishAsync(string topic, string payload, CancellationToken cancellationToken);
}
