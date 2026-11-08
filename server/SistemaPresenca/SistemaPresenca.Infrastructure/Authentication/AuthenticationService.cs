using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Services;
using SistemaPresenca.Domain.Models;
using SistemaPresenca.Infrastructure.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SistemaPresenca.Infrastructure.Authentication;

public class AuthenticationService(
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService,
    ILogger<AuthenticationService> logger)
    : IAuthenticationService
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public Result<string> Authenticate(User user, string password)
    {
        var comparePasswordResult = passwordHasher.VerifyHashedPassword(user, user.Password, password);

        if (comparePasswordResult == PasswordVerificationResult.Failed)
        {
            logger.LogInformation("Authentication failed because incorrect password for user with email: {Email}", user.Email);

            return Result<string>.Failure(AuthenticationErrors.IncorrectCredentials);
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var accessToken = tokenService.GenerateAccessToken(claims);

        var token = _tokenHandler.WriteToken(accessToken);

        return Result<string>.Success(token);
    }
}