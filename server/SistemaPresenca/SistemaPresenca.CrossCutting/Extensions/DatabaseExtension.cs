using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.CrossCutting.Models;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, PostgresSettings postgresSettings)
    {
        services.AddDbContext<SistemaPresencaDbContext>(config =>
        {
            config.UseNpgsql(postgresSettings.ConnectionString);
            config.EnableSensitiveDataLogging();
        });

        return services;
    }
}
