using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed record UpdateSubjectCommand(Guid Id, JsonPatchDocument<UpdateSubjectRequest> PatchRequest) : ICommand<Result>;