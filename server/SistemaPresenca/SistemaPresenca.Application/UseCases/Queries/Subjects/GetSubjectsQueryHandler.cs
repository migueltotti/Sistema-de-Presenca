using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Subjects;

public sealed class GetSubjectsQueryHandler(ISubjectRepository subjectRepository) : IQueryHandler<GetSubjectsQuery, IEnumerable<GetSubjectResponse>>
{
    public async Task<IEnumerable<GetSubjectResponse>> HandleAsync(GetSubjectsQuery message, CancellationToken cancellationToken = default)
    {
        var filter = new SubjectFilters.Builder()
            .WithIds(message.Request.Ids)
            .WithNames(message.Request.Names)
            .WithCodes(message.Request.Codes)
            .WithMajorIds(message.Request.MajorIds)
            .WithProfessorIds(message.Request.ProfessorIds)
            .Build();

        var subjects = await subjectRepository.GetSubjectsAsync(filter, cancellationToken);

        return subjects.Select(x => x.ToResponse());
    }
}
