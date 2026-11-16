namespace SistemaPresenca.Application.Responses.Subjects;

public sealed record GetSubjectResponse(
    Guid Id,
    string Name,
    string Code,
    int TotalClasses,
    Guid MajorId,
    IEnumerable<GetSubjectProfessorResponse>? Professors
);

public sealed record GetSubjectProfessorResponse(
    Guid Id,
    string Name,
    string Email,
    string RegistrationId,
    string Cpf);
