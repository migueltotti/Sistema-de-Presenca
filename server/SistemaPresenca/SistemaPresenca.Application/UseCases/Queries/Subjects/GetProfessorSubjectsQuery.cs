using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Responses.Subjects;

namespace SistemaPresenca.Application.UseCases.Queries.Subjects;

public sealed record GetProfessorSubjectsQuery(Guid ProfessorId) : IQuery<IEnumerable<GetSubsjectResponse>>;