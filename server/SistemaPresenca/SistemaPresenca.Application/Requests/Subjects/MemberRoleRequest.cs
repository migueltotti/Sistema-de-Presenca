using System.Text.Json.Serialization;

namespace SistemaPresenca.Application.Requests.Subjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MemberRoleRequest
{
    Professor,
    Student
}