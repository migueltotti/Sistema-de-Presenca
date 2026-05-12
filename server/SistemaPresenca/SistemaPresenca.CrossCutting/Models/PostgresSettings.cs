namespace SistemaPresenca.CrossCutting.Models;

public record PostgresSettings
{
    public required string ConnectionString { get; set; }
}
