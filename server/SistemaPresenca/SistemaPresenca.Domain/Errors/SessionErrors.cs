using Mattioli.Configurations.Models;

namespace SistemaPresenca.Domain.Errors;

public static class SessionErrors
{
    public static Error NotFound => new(
        "SessionErrors.NotFound",
        "Session with provided id not found.");
}
