using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.Application.UseCases.Commands.Sessions;

namespace SistemaPresenca.CrossCutting;

public static class MediatorExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        return services.AddLiteBus(config =>
        {
            config.AddCommandModule(module => module
                .RegisterFromAssembly(typeof(StartSessionCommand).Assembly));
        });
    }
}
