using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class OpenApiSpecificationExtension
{
    public static IServiceCollection AddApiSpecification(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddScalarTransformers();
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes!.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' and then your valid token."
                });
                return Task.CompletedTask;
            });
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

            options.WithTitle("Sistema de Presença | Reference");
            options.WithClassicLayout();
            options.ExpandAllTags();
            options.AddPreferredSecuritySchemes("Bearer");
        });
    }
}
