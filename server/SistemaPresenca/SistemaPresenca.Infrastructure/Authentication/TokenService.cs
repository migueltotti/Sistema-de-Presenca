using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaPresenca.Infrastructure.Interfaces;
using SistemaPresenca.Infrastructure.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaPresenca.Infrastructure.Authentication;

public class TokenService(IOptions<JwtSettings> settings) : ITokenService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var privateKey = Encoding.UTF8.GetBytes(settings.Value.SecretKey);

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(settings.Value.TokenValidityInMinutes),
            Audience = settings.Value.Audience,
            Issuer = settings.Value.Issuer,
            SigningCredentials = signingCredentials
        };

        var token = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return token;
    }
}