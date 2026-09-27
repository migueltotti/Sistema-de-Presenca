using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public sealed record CreateUserCommand(CreateUserRequest Request) : ICommand<Result>;