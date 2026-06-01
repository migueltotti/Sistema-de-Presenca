using SistemaPresenca.Domain.Entities;

namespace SistemaPresenca.Domain.Interfaces.Repositories;

public interface ISubjectRepository : IBaseRepository<Subject>
{
    Task<Subject?> GetWithStudents(Guid subjectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subject>> GetByProfessorId(Guid professorId, CancellationToken cancellationToken = default);
}
