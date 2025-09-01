using Api.Filters;
using Application.Services.Interfaces;
using Domain.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Authorization Endpoint")]
[Route("api/auth")]
public class AuthController(IAuthService service) : ApiBaseAuthController
{
    [HttpPost("login")]
    [EndpointName("get token")]
    [EndpointSummary("Authenticates a user and issues a JWT token.")]
    [ProducesResponseType(typeof(ApiResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var authResponse = await service.LoginAsync(login);
        return Ok(authResponse);
    }
}