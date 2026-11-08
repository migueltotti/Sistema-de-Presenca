using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Interfaces.Services;
using SistemaPresenca.Infrastructure.Authentication;
using SistemaPresenca.Infrastructure.Interfaces;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}