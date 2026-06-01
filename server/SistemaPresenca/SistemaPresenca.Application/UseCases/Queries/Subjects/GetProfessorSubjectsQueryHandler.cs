using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Subjects;

public sealed class GetProfessorSubjectsQueryHandler(
    ISubjectRepository subjectRepository) : IQueryHandler<GetProfessorSubjectsQuery, IEnumerable<GetSubsjectResponse>>
{
    public async Task<IEnumerable<GetSubsjectResponse>> HandleAsync(GetProfessorSubjectsQuery message, CancellationToken cancellationToken = default)
    {
        var professorSubjects = await subjectRepository.GetByProfessorId(message.ProfessorId, cancellationToken);

        return professorSubjects.Select(x => x.ToResponse());
    }
}
