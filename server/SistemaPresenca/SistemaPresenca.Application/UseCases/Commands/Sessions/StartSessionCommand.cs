using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;
using SistemaPresenca.Application.Requests.Session;
using SistemaPresenca.Application.Responses.Session;

namespace SistemaPresenca.Application.UseCases.Commands.Sessions;

public record StartSessionCommand(StartSessionRequest Request) : ICommand<Result<StartSessionResponse>>;
