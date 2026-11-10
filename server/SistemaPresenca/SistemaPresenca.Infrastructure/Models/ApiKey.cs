using AspNetCore.Authentication.ApiKey;
using System.Security.Claims;

namespace SistemaPresenca.Infrastructure.Models;

public class ApiKey(string key, string owner, IReadOnlyCollection<Claim> claims)
    : IApiKey
{
    public string Key { get; } = key;
    public string OwnerName { get; } = owner;
    public IReadOnlyCollection<Claim> Claims { get; } = claims;
}