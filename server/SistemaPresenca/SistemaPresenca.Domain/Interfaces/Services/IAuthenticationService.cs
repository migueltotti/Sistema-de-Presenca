using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Interfaces.Services;

public interface IAuthenticationService
{
    Result<string> Authenticate(User user, string password);
}
