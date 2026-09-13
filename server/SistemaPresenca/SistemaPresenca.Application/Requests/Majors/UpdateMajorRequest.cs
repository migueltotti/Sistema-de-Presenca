using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using SistemaPresenca.Application.Responses.Majors;

namespace SistemaPresenca.Application.Requests.Majors;

public sealed record UpdateMajorRequest(JsonPatchDocument<GetMajorsResponse> Document);