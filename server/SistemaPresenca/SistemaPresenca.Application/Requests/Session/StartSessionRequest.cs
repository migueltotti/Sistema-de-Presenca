namespace SistemaPresenca.Application.Requests.Session;

public record StartSessionRequest(
    Guid ProfessorId,
    Guid SubjectId,
    int NumberOfClasses
);
