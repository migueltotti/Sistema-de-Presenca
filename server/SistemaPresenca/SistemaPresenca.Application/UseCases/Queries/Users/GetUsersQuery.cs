using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Application.Responses.Users;

namespace SistemaPresenca.Application.UseCases.Queries.Users;

public sealed record GetUsersQuery(GetUsersRequest Request) : IQuery<IEnumerable<GetUserResponse>>;