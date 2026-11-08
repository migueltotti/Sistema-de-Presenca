using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Errors;

public static class AuthenticationErrors
{
    public static Error IncorrectCredentials => new(
        "AuthenticationError.IncorrectCredentials",
        "Incorrect email or password.");
}