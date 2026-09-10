using LiteBus.Commands.Abstractions;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.Requests.Session;

public sealed record RegisterAttendanceRequest(string StudentTagId);
