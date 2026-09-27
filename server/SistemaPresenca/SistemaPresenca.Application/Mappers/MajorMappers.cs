using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class MajorMappers
{
    public static Major ToEntity(this CreateMajorRequest request)
    {
        return new Major(
            request.Name,
            request.Code,
            Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c")
        );
    }

    public static GetMajorsResponse ToResponse(this Major major)
    {
        return new GetMajorsResponse(
            major.Id,
            major.Name,
            major.Code,
            major.CreatedAt,
            major.CreatedByAdminId
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
