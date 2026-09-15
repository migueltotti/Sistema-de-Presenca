namespace SistemaPresenca.Application.Requests.Subjects;

public sealed record CreateSubjectRequest(
    string Name,
    string Code,
    int TotalClasses,
    Guid MajorId
);