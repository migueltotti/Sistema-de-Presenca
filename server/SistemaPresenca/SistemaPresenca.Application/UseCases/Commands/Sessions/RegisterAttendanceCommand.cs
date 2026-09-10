using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record RegisterAttendanceCommand(Guid SessionId, string StudentTagId) : ICommand<Result>;
