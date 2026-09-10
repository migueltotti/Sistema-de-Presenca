namespace SistemaPresenca.Application.Requests.Majors;

public sealed record CreateMajorRequest(
    string Name,
    string Code
);
