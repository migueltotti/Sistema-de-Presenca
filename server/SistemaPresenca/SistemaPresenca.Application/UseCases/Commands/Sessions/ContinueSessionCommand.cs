using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record ContinueSessionCommand(Guid SessionId, string ProfessorTagId) : ICommand<Result>;
