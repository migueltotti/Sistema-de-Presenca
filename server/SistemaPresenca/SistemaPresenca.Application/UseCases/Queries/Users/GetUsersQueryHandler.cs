using LiteBus.Queries.Abstractions;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Users;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Users;

public sealed class GetUsersQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUsersQuery, IEnumerable<GetUserResponse>>
{
    public async Task<IEnumerable<GetUserResponse>> HandleAsync(GetUsersQuery message, CancellationToken cancellationToken = default)
    {
        var filter = new UserFilters.Builder()
            .WithIds(message.Request.Ids)
            .WithNames(message.Request.Names)
            .WithEmails(message.Request.Emails)
            .WithRegistrationIds(message.Request.RegistrationIds)
            .WithCpfs(message.Request.Cpfs)
            .WithTagIds(message.Request.TagIds)
            .WithRoles(message.Request.UserRoles)
            .Build();

        var users = await userRepository.GetUsersAsync(filter, cancellationToken);

        return users.Select(x => x.ToResponse());
    }
}
