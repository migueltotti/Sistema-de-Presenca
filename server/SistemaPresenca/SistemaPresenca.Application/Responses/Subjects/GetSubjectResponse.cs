namespace SistemaPresenca.Application.Responses.Subjects;

public sealed record GetSubjectResponse(
    Guid Id,
    string Name,
    string Code,
    int TotalClasses,
    Guid MajorId,
    IEnumerable<Guid>? ProfessorIds
);
