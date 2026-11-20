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

    public static Error StudentsRequiredToCreateSession => new(
        "SubjectErrors.StudentsRequiredToCreateSession",
        "Subject must have at least one student to create a session.");

    public static Error UserAlreadyAssigned(string userRole) => new(
        $"SubjectErrors.{userRole}AlreadyAssigned",
        $"{userRole} is already assigned to Subject.");

    public static Error InvalidUpdateRequest(string description) => new(
        "SubjectErrors.InvalidUpdateRequest",
        description);
}
