using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed record DeleteSubjectCommand(Guid Id) : ICommand<Result>;