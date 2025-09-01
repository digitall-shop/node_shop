namespace Domain.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }

    public string Type { get; set; } = "Bearer";
    
    public int ExpiresIn { get; set; }
}