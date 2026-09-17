namespace SistemaPresenca.Application.Requests.Subjects;

public record UpdateSubjectRequest(
    string Name,
    string Code,
    int TotalClasses);