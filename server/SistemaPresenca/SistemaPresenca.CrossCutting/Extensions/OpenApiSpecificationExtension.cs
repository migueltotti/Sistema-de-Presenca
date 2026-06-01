using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class OpenApiSpecificationExtension
{
    public static IServiceCollection AddApiSpecification(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddScalarTransformers();
        });

        return services;
    }

    public static void UseSpecification(this IEndpointRouteBuilder app)
    {
        app.MapScalarApiReference(options =>
        {
            options.DarkMode = true;
            options.HideDarkModeToggle = true;
            options.HideClientButton = true;
            options.HideModels = true;
            options.HideSearch = true;
            options.Servers = [];

            options.WithTitle("Sistema de presença | Reference");
            options.WithClassicLayout();
            options.ExpandAllTags();
        });
    }
}
