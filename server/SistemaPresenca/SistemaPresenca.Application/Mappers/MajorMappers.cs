using SistemaPresenca.Application.Requests.Majors;
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
}
