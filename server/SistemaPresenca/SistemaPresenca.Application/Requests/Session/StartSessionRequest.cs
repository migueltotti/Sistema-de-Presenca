namespace SistemaPresenca.Application.Requests.Session;

public record StartSessionRequest(
    string ProfessorTagId,
    Guid SubjectId,
    Guid RoomId,
    int NumberOfClasses
);
