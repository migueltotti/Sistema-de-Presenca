using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Errors;

public static class SubjectErrors
{
    public static Error NotFound => new(
        "SubjectErrors.NotFound",
        "Subject with provided id not found.");

    public static Error CodeAlreadyExists => new(
        "SubjectErrors.CodeAlreadyExists",
        "Subject with the same code already exists for the provided Major.");

    public static Error ProfessorMismatch => new(
        "SubjectErrors.ProfessorMismatch",
        "Professor does not teach the provided subject.");
}
