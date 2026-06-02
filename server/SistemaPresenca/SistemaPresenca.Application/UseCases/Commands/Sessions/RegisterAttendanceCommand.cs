using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record RegisterAttendanceCommand(Guid SessionId, string StudentTagId) : ICommand<Result>;
