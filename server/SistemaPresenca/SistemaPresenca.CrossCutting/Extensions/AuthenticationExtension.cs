using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaPresenca.Infrastructure.Models;
using System.Text;
using System.Text.Json;

namespace SistemaPresenca.CrossCutting.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddSingleton(Options.Create(jwtSettings));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearerAuthentication(jwtSettings);

        services.AddRoleAuthorization();

        return services;
    }

    private static AuthenticationBuilder AddJwtBearerAuthentication(this AuthenticationBuilder builder,
        JwtSettings jwtSettings)
    {
        return builder.AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = jwtSettings.Audience,
                ValidIssuer = jwtSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    var detail = context.AuthenticateFailure switch
                    {
                        SecurityTokenExpiredException => "Expired Token.",
                        SecurityTokenInvalidSignatureException => "Invalid Signature.",
                        null => "You must provide a valid Bearer token.",
                        _ => "Invalid token."
                    };

                    await WriteUnauthorizedAsync(context.HttpContext, JwtBearerDefaults.AuthenticationScheme, detail);
                }
            };
        });
    }

    public static async Task WriteUnauthorizedAsync(HttpContext httpContext, string scheme, string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["scheme"] = scheme;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }

    private static IServiceCollection AddRoleAuthorization(this IServiceCollection services)
    {
        return services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("AdminOrProfessor", policy => policy.RequireRole("Admin", "Professor"));
        });
    }
}