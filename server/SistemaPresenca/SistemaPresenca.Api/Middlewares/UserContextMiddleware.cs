using Microsoft.AspNetCore.Authorization;
using SistemaPresenca.Application.Interfaces;
using System.Security.Claims;

namespace SistemaPresenca.Api.Middlewares;

public class UserContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var userContextProvider = context.RequestServices.GetRequiredService<IUserContextProvider>();

        var endpoint = context.GetEndpoint();
        var requiresAuthorization = endpoint?.Metadata.GetOrderedMetadata<IAuthorizeData>().Any() == true;

        if (!requiresAuthorization)
        {
            await _next(context);
            return;
        }

        Guid.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId);
        var userEmail = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

        userContextProvider.SetUserId(userId);
        userContextProvider.SetUserEmail(userEmail);

        await _next(context);
        return;
    }
}
