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

    public static Major ToEntity(this GetMajorsResponse response)
    {
        return new Major(
            response.Name,
            response.Code,
            response.CreatedByAdminId
        )
        {
            Id = response.Id,
            CreatedAt = response.CreatedAt
        };
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
}
