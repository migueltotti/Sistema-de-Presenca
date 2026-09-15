using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;

namespace SistemaPresenca.Application.UseCases.Queries.Majors;

public sealed record GetMajorsQuery(GetMajorsRequest Request) : IQuery<IEnumerable<GetMajorsResponse>>;