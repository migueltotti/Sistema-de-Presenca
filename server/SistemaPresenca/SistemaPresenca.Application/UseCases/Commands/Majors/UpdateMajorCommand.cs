using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SistemaPresenca.Application.Requests.Majors;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Majors;

public sealed record UpdateMajorCommand(Guid Id, JsonPatchDocument<UpdateMajorRequest> PatchRequest) : ICommand<Result>;
