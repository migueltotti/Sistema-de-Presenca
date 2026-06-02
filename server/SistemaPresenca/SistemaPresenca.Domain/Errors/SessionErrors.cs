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

    public static Error StudentNotFound => new(
        "SessionErrors.StudentNotFound",
        "Student with provided tag id not found at session."
    );

    public static Error StudentAttendanceAlreadyRegistered => new(
        "SessionErrors.StudentAttendanceAlreadyRegistered",
        "Student with provided tag id already has attendance registered for this session."
    );
}
