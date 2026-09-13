namespace SistemaPresenca.Application.Requests.Majors;

public class GetMajorsRequest
{
    public IEnumerable<Guid>? Ids { get; set; }
    public IEnumerable<string>? Names { get; set; }
    public IEnumerable<string>? Codes { get; set; }
}