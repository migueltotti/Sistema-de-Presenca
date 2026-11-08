using Microsoft.Extensions.Logging;
using MQTTnet;
using SistemaPresenca.Application.Interfaces;

namespace SistemaPresenca.Infrastructure.Mqtt;

public sealed class MqttProducer(
    IMqttClient mqttClient,
    ILogger<MqttProducer> logger) : IMqttProducer
{
    public async Task PublishAsync(string topic, string payload, CancellationToken cancellationToken)
    {
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await mqttClient.PublishAsync(message, cancellationToken);

        logger.LogInformation("Published message to topic {Topic}: {Payload}", topic, payload);
    }
}
