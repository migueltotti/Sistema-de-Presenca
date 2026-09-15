using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Messaging;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.Application.UseCases.Commands.Sessions;
using SistemaPresenca.Application.UseCases.Queries.Subjects;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class MediatorExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        return services.AddLiteBus(config =>
        {
            config.AddMessaging(_ => { });

            config.AddCommands(module => module
                .RegisterFromAssembly(typeof(StartSessionCommand).Assembly));

            config.AddQueries(module => module
                .RegisterFromAssembly(typeof(GetSubjectsQuery).Assembly));
        });
    }
}
