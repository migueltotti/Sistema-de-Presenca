using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public sealed record EndSessionCommand(Guid SessionId, string ProfessorTagId) : ICommand<Result>;
