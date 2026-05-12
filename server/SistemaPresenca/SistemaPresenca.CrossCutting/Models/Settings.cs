namespace SistemaPresenca.CrossCutting.Models;

public interface ISettings
{
    public PostgresSettings PostgresSettings { get; }
}

public class Settings : ISettings
{
    public required PostgresSettings PostgresSettings { get; set; }
}
