using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SistemaPresenca.Application.Requests.Rooms;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Rooms;

public sealed record UpdateRoomCommand(Guid Id, JsonPatchDocument<UpdateRoomRequest> PatchRequest) : ICommand<Result>;
