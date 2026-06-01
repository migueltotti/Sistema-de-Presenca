using Microsoft.EntityFrameworkCore;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Enums;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Infrastructure.Context;

namespace SistemaPresenca.Infrastructure.Repositories;

public class UserRepository(SistemaPresencaDbContext context) : BaseRepository<User>(context), IUserRepository
{
}
