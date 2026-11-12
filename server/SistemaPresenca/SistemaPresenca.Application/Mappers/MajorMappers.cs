using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class MajorMappers
{
    public static Major ToEntity(this CreateMajorRequest request, Guid createdByAdminId)
    {
        return new Major(
            request.Name,
            request.Code,
            createdByAdminId
        );
    }

    public static GetMajorsResponse ToResponse(this Major major)
    {
        return new GetMajorsResponse(
            major.Id,
            major.Name,
            major.Code,
            major.CreatedAt,
            major.CreatedById
        );
    }

    public static UpdateMajorRequest ToUpdateRequest(this Major major)
    {
        return new UpdateMajorRequest(
            major.Name,
            major.Code
        );
    }

    public static void UpdateEntity(this UpdateMajorRequest request, Major major)
    {
        major.Name = request.Name;
        major.Code = request.Code;
    }
}
