using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Filters;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface ISubjectRepository : IBaseRepository<Subject>
{
    Task<IEnumerable<Subject>> GetSubjectsAsync(SubjectFilters filters, CancellationToken cancellationToken = default);
}
