namespace SistemaPresenca.Application.Requests.Subjects;

public sealed class GetSubjectsRequest
{
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Codes { get; set; }
    public IEnumerable<Guid>? MajorIds { get; set; }
    public IEnumerable<Guid>? ProfessorIds { get; set; }
}