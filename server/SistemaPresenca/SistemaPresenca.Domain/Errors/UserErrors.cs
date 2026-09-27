using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Errors;

public static class UserErrors
{
    public static Error NotFound => new(
        "UserErrors.NotFound",
        "User with provided id not found.");

    public static Error EmailOrCpfAlreadyExists => new(
        "UserErrors.EmailOrCpfAlreadyExists",
        "User with same email or cpf already exists.");

    public static Error EmailOrTagIdAlreadyExists => new(
        "UserErrors.EmailOrTagIdAlreadyExists",
        "User with same email or tagId already exists.");

    public static readonly Error ProfessorNotFound = new(
        "UserErrors.ProfessorNotFound",
        "Professor with provided tag id not found."
    );

    public static Error StudentNotFound => new(
        "UserErrors.StudentNotFound",
        "Student with provided tag id not found."
    );

    public static Error InvalidUpdateRequest(string description) => new(
        "UserErrors.InvalidUpdateRequest",
        description);
}
