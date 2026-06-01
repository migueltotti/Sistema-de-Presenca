using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class SubjectMapper
{
    public static GetSubsjectResponse ToResponse(this Subject subject)
    {
        return new GetSubsjectResponse
        (
            subject.Id,
            subject.Name,
            subject.Code,
            subject.TotalClasses,
            subject.MajorId,
            subject.ProfessorId
        );  
    }
}
