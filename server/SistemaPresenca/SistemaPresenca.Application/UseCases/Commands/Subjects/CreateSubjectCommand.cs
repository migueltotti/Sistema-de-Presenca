using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed record CreateSubjectCommand(CreateSubjectRequest Request) : ICommand<Result>;