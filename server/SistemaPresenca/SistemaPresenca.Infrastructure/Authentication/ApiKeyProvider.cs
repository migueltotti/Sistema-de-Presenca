using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Options;
using SistemaPresenca.Infrastructure.Models;

namespace SistemaPresenca.Infrastructure.Authentication;

public class ApiKeyProvider(IOptions<ApiKeySettings> apiKeySettings) : IApiKeyProvider
{
    public Task<IApiKey?> ProvideAsync(string key)
    {
        if (!apiKeySettings.Value.ApiKeys.Contains(key))
        {
            return Task.FromResult<IApiKey?>(null);
        }

        var apiKey = new ApiKey(key, "embedded-system", []);

        return Task.FromResult<IApiKey?>(apiKey);
    }
}
