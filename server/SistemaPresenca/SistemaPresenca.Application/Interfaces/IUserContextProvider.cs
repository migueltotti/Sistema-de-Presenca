using SistemaPresenca.Application.Models;
using SistemaPresenca.Domain.Enums;

namespace SistemaPresenca.Application.Interfaces;

public interface IUserContextProvider
{
    public UserContextModel User { get; }
}
