using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Identity.Data;
using SistemaPresenca.Application.Responses.Users;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public record LoginCommand(LoginRequest Request) : ICommand<Result<LoginResponse>>;