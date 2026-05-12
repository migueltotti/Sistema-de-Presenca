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
        }

        return settings!;
    }

    private static string GetEnvOrDefault(string key, string? fallback)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return string.IsNullOrWhiteSpace(value) ? fallback ?? "" : value;
    }
}
