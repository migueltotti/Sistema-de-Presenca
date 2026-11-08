using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SistemaPresenca.Infrastructure.Interfaces;

public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims);
}