using LiteBus.Queries.Abstractions;
using SistemaPresenca.Domain.Models;
using SistemaPresenca.Application.Responses.Subjects;

namespace SistemaPresenca.Application.UseCases.Queries.Subjects;

public sealed record GetProfessorSubjectsQuery(string ProfessorTagId) : IQuery<Result<IEnumerable<GetSubsjectResponse>>>;