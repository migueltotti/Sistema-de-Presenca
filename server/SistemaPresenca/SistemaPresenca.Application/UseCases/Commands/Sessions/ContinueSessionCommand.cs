using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record ContinueSessionCommand(Guid SessionId, string ProfessorTagId) : ICommand<Result>;
