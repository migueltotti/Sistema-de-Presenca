using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SistemaPresenca.CrossCutting.Models;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class SettingsExtension
{
    public static Settings GetApplicationSettings(this IConfiguration configuration, IHostEnvironment env)
    {
        var settings = configuration.GetSection("Settings").Get<Settings>();

        if (!env.IsDevelopment())
        {
            settings!.PostgresSettings.ConnectionString = GetEnvOrDefault("PostgreSql_ConnectionString", settings.PostgresSettings.ConnectionString);
            settings.MqttSettings.Host = GetEnvOrDefault("Mqtt_Host", settings.MqttSettings.Host);
            settings.MqttSettings.Port = int.Parse(GetEnvOrDefault("Mqtt_Port", settings.MqttSettings.Port.ToString()));
            settings.MqttSettings.Username = GetEnvOrDefault("Mqtt_Username", settings.MqttSettings.Username);
            settings.MqttSettings.Password = GetEnvOrDefault("Mqtt_Password", settings.MqttSettings.Password);
            settings.MqttSettings.ClientId = GetEnvOrDefault("Mqtt_ClientId", settings.MqttSettings.ClientId);
            settings.JwtSettings.SecretKey = GetEnvOrDefault("Jwt_SecretKey", settings.JwtSettings.SecretKey);
            settings.JwtSettings.TokenValidityInMinutes = int.Parse(GetEnvOrDefault("Jwt_TokenValidityInMinutes", settings.JwtSettings.TokenValidityInMinutes.ToString()));
            settings.JwtSettings.Issuer = GetEnvOrDefault("Jwt_Issuer", settings.JwtSettings.Issuer);
            settings.JwtSettings.Audience = GetEnvOrDefault("Jwt_Audience", settings.JwtSettings.Audience);
        }

        return settings!;
    }

    private static string GetEnvOrDefault(string key, string? fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback ?? "" : value;
    }
}
