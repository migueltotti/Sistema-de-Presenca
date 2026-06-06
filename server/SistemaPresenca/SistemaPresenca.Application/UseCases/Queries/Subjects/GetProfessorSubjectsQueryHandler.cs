using LiteBus.Queries.Abstractions;
using Mattioli.Configurations.Models;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Subjects;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Subjects;

public sealed class GetProfessorSubjectsQueryHandler(
    ISubjectRepository subjectRepository,
    IUserRepository userRepository,
    ILogger<GetProfessorSubjectsQueryHandler> logger) : IQueryHandler<GetProfessorSubjectsQuery, Result<IEnumerable<GetSubsjectResponse>>>
{
    public async Task<Result<IEnumerable<GetSubsjectResponse>>> HandleAsync(GetProfessorSubjectsQuery query, CancellationToken cancellationToken = default)
    {
        var professor = await userRepository.GetAsync(x => x.TagId == query.ProfessorTagId && x.Role == UserRole.Professor, cancellationToken);
        if (professor is null)
        {
            logger.LogError("Professor with tagId {ProfessorTagId} - not found", query.ProfessorTagId);
            return Result<IEnumerable<GetSubsjectResponse>>.Failure(UserErrors.ProfessorNotFound);
        }

        var professorSubjects = await subjectRepository.GetByProfessorId(professor.Id, cancellationToken);

        return Result<IEnumerable<GetSubsjectResponse>>.Success(professorSubjects.Select(x => x.ToResponse()));
    }
}
