using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Application.Responses.Majors;
using SistemaPresenca.Domain.Filters;
using SistemaPresenca.Domain.Interfaces.Repositories;

namespace SistemaPresenca.Application.UseCases.Queries.Majors;

public sealed class GetMajorsCommandHandler(IMajorRepoitory majorRepoitory) : ICommandHandler<GetMajorsCommand, IEnumerable<GetMajorsResponse>>
{
    public async Task<IEnumerable<GetMajorsResponse>> HandleAsync(GetMajorsCommand message, CancellationToken cancellationToken = default)
    {
        var filter = new MajorFilters.Builder()
            .WithIds(message.Request.Ids)
            .WithNames(message.Request.Names)
            .WithCode(message.Request.Codes)
            .Build();

        var majors = await majorRepoitory.GetMajorsAsync(filter, cancellationToken);

        return majors.Select(x => x.ToResponse());
    }
}
