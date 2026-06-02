using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record EndSessionCommand(Guid SessionId, string ProfessorTagId) : ICommand<Result>;
