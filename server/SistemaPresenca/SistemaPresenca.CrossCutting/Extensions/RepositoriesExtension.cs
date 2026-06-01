using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Repositories;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class RepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();

        return services;
    }
}
