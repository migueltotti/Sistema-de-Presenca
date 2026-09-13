using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public sealed record UpdateMajorCommand(Guid Id, UpdateMajorRequest PatchRequest) : ICommand<Result>;
