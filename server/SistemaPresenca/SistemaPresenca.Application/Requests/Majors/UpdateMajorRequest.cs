namespace SistemaPresenca.Application.Requests.Majors;

public sealed record UpdateMajorRequest(
    string Name,
    string Code);