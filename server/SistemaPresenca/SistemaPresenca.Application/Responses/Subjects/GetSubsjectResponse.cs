namespace SistemaPresenca.Application.Responses.Subjects;

public sealed record GetSubsjectResponse(
    Guid Id,
    string Name,
    string Code,
    int TotalClasses,
    Guid MajorId,
    Guid? ProfessorId
);
