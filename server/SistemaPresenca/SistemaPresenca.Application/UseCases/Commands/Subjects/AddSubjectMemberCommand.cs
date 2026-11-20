using LiteBus.Commands.Abstractions;
using SistemaPresenca.Application.Requests.Subjects;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Subjects;

public sealed record AddSubjectMemberCommand(Guid SubjectId, Guid MemberId, MemberRoleRequest MemberRole) : ICommand<Result>;