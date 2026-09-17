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

    public static GetSubjectResponse ToResponse(this Subject subject)
    {
        return new GetSubjectResponse(
            subject.Id,
            subject.Name,
            subject.Code,
            subject.TotalClasses,
            subject.MajorId,
            subject.ProfessorIds
        );  
    }

    public static UpdateSubjectRequest ToUpdateRequest(this Subject subject)
    {
        return new UpdateSubjectRequest(
            subject.Name,
            subject.Code,
            subject.TotalClasses
        );
    }

    public static void UpdatedEntity(this UpdateSubjectRequest request, Subject subject)
    {
        subject.Name = request.Name;
        subject.Code = request.Code;
        subject.TotalClasses = request.TotalClasses;
    }
}
