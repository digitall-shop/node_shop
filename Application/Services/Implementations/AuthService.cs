using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Contract;
using Domain.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using User = Domain.Entities.User;

namespace Application.Services.Implementations;

public class AuthService(
    IAsyncRepository< User ,long> repository
    ,ILogger<IAuthService> logger
    ,IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto login)
    {    
        var user = await repository.GetSingleAsync(u => u.UserName == login.UserName)??
            throw new UnauthorizedAccessException("invalid username or password");
    
        var isPasswordValid = PasswordHasher.VerifyPassword(user.Password, login.Password);
        
        if (!isPasswordValid)
        {
            logger.LogWarning("Login failed: Invalid password for user {Username}.", login.UserName);
            throw new UnauthorizedAccessException("invalid password for username");
        }
        
        var accessToken =JwtHelpers.GenerateJwtToken(user,configuration);
        
        return new AuthResponseDto
        {
            Token = accessToken,
            Type = "Bearer",
            ExpiresIn = configuration.GetValue("Jwt:ExpirationMinutes", 5000) * 60
        };
    }
}