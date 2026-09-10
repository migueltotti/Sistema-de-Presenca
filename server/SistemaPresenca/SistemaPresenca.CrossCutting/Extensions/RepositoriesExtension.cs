using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Repositories;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class RepositoriesExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMajorRepoitory, MajorRepoitory>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        return services;
    }
}
