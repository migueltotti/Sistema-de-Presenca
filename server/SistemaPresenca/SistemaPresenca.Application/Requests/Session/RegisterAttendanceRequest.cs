using LiteBus.Commands.Abstractions;
using Mattioli.Configurations.Models;

namespace SistemaPresenca.Application.Requests.Session;

public sealed record RegisterAttendanceRequest(string StudentTagId);
