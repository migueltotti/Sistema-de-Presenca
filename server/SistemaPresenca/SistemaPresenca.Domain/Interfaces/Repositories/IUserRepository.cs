using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<IEnumerable<User>> GetUsersAsync(UserFilters filters, CancellationToken cancellationToken);
}
