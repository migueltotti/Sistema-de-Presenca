using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Application.Responses.Majors;

namespace SistemaPresenca.Application.UseCases.Queries.Majors;

public sealed record GetMajorsCommand(GetMajorsRequest Request) : ICommand<IEnumerable<GetMajorsResponse>>;