using Mattioli.Configurations.Models;

namespace SistemaPresenca.Domain.Errors;

public static class SessionErrors
{
    public static Error NotFound => new(
        "SessionErrors.NotFound",
        "Session with provided id not found.");

    public static Error ProfessorMismatch => new(
        "SessionErrors.ProfessorMismatch",
        "Professor with provided id is not assigned to the session.");

    public static Error AlreadyFinished => new(
        "SessionErrors.AlreadyFinished",
        "Session with provided id is already finished.");
}
