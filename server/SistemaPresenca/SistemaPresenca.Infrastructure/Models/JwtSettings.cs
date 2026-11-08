namespace SistemaPresenca.Infrastructure.Models;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public required double TokenValidityInMinutes { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}