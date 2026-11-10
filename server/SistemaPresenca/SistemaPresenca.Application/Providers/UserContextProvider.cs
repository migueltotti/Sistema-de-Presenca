using Microsoft.AspNetCore.Http;
using SistemaPresenca.Application.Interfaces;
using SistemaPresenca.Application.Models;
using SistemaPresenca.Domain.Enums;
using System.Security.Claims;

namespace SistemaPresenca.Application.Providers;

public sealed class UserContextProvider(IHttpContextAccessor httpContextAccessor) : IUserContextProvider
{
    private UserContextModel? _user;

    public UserContextModel User => _user ??= Build();

    private UserContextModel Build()
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var model = new UserContextModel();

        if (principal?.Identity?.IsAuthenticated != true)
            return model;

        if (Guid.TryParse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id))
            model.Id = id;

        model.Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        if (Enum.TryParse<UserRole>(principal.FindFirst(ClaimTypes.Role)?.Value, true, out var role))
            model.Role = role;

        return model;
    }
}
