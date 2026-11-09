using Microsoft.AspNetCore.Authorization;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Domain.Enums;
using System.Security.Claims;

namespace SistemaPresenca.Api.Middlewares;

public class UserContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var requiresAuthorization =
            endpoint?.Metadata.GetMetadata<IAuthorizeData>() is not null &&
            endpoint.Metadata.GetMetadata<IAllowAnonymous>() is null;

        if (!requiresAuthorization || context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var userContextProvider = context.RequestServices.GetRequiredService<IUserContextProvider>();

        Guid.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId);
        var userEmail = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        var userRole = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

        userContextProvider.SetUserId(userId);
        userContextProvider.SetUserEmail(userEmail);
        userContextProvider.SetUserRole(userRole != null ? Enum.Parse<UserRole>(userRole) : null);

        await _next(context);
        return;
    }
}
