using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public sealed record DeleteUserCommand(Guid Id) : ICommand<Result>;