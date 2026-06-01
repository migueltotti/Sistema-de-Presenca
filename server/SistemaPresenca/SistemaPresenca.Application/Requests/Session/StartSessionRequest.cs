namespace SistemaPresenca.Application.Requests.Session;

public record StartSessionRequest(
    string ProfessorTagId,
    Guid SubjectId,
    int NumberOfClasses
);
