using Domain.DTOs.Auth;

namespace Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto login);
}