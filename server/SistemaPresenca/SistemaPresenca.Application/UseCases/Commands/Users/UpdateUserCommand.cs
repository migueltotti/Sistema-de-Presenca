using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SistemaPresenca.Application.Requests.Users;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public sealed record UpdateUserCommand(Guid Id, JsonPatchDocument<UpdateUserRequest> PatchRequest) : ICommand<Result>;