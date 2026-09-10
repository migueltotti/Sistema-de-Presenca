using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SistemaPresenca.Application.Validators.Majors;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class ValidatorsExtension
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(CreateMajorRequestValidator));
        services.AddFluentValidationAutoValidation(config =>
        {
            config.DisableBuiltInModelValidation = true;
            config.EnableBodyBindingSourceAutomaticValidation = true;
        });

        return services;
    }
}
