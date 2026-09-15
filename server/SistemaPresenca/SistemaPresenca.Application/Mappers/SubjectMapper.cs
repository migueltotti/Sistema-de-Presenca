using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Application.Mappers;

public static class SubjectMapper
{
    public static Subject ToEntity(this CreateSubjectRequest request)
    {
        return new Subject(
            request.Name,
            request.Code,
            request.TotalClasses,
            request.MajorId,
            Guid.Parse("95934e86-cda1-44d0-831c-0aa42892650c")
        );
    }

    public static GetSubsjectResponse ToResponse(this Subject subject)
    {
        return new GetSubsjectResponse
        (
            subject.Id,
            subject.Name,
            subject.Code,
            subject.TotalClasses,
            subject.MajorId,
            subject.ProfessorIds
        );  
    }
}
