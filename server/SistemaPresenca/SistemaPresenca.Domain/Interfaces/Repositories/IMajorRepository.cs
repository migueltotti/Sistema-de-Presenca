using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface IMajorRepository : IBaseRepository<Major>
{
    Task<IEnumerable<Major>> GetMajorsAsync(MajorFilters filters, CancellationToken cancellationToken);
}
