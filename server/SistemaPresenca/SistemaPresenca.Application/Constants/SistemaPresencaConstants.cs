using System.Text.Json;

namespace SistemaPresenca.Application.Constants;

public static class SistemaPresencaConstants
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
