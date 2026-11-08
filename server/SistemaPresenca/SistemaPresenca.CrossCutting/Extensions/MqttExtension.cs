using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.CrossCutting.Models;
using SistemaPresenca.Infrastructure.Mqtt;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class MqttExtension
{
    public static IServiceCollection AddMqtt(this IServiceCollection services, MqttSettings settings)
    {
        services.AddSingleton(sp =>
        {
            var mqttFactory = new MqttClientFactory();
            var mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(settings.Host, settings.Port)
                .WithCredentials(settings.Username, settings.Password)
                .WithClientId(settings.ClientId)
                .Build();

            mqttClient.ConnectAsync(mqttClientOptions).GetAwaiter().GetResult();

            return mqttClient;
        });

        services.AddSingleton<IMqttProducer, MqttProducer>();

        return services;
    }
}
