using SistemaPresenca.Infrastructure.Models;

namespace SistemaPresenca.CrossCutting.Models;

public interface ISettings
{
    public PostgresSettings PostgresSettings { get; }
    public MqttSettings MqttSettings { get; }
    public JwtSettings JwtSettings { get; }
    public ApiKeySettings ApiKeySettings { get; }
}

public class Settings : ISettings
{
    public required PostgresSettings PostgresSettings { get; set; }
    public required MqttSettings MqttSettings { get; set; }
    public required JwtSettings JwtSettings { get; set; }
    public required ApiKeySettings ApiKeySettings { get; set; }
}
