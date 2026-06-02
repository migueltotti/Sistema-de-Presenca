using Mattioli.Configurations.Models;

namespace SistemaPresenca.Domain.Errors;

public static class UserErrors
{
    public static readonly Error ProfessorNotFound = new(
        "UserErrors.ProfessorNotFound",
        "Professor with provided tag id not found."
    );

    public static Error StudentNotFound => new(
        "UserErrors.StudentNotFound",
        "Student with provided tag id not found."
    );
}
