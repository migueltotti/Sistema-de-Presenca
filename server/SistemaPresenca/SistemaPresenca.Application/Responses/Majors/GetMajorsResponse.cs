namespace SistemaPresenca.Application.Responses.Majors;

public sealed record GetMajorsResponse(Guid Id, string Name, string Code, DateTime CreatedAt, Guid? CreatedByAdminId);