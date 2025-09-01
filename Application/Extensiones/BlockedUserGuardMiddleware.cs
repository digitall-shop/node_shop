
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Services.Interfaces;
using Domain.Contract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Extensiones;

public sealed class BlockedUserGuardMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly string[] _whitelistPrefixes =
    {
        "/swagger", "/openapi", "/scalar", "/health", "/EasyUi",        
        "/api/auth",                                                   
        "/api/telegram"                                          
    };

    public async Task Invoke(HttpContext ctx, IUserContextService userCtx, IUserService userService, ILogger<BlockedUserGuardMiddleware> logger)
    {
        var path = ctx.Request.Path.Value ?? string.Empty;

        if (IsWhitelisted(path))
        {
            await next(ctx);
            return;
        }

        if (ctx.User?.Identity?.IsAuthenticated != true)
        {
            await next(ctx);
            return;
        }

        try
        {
            
            var user   = await userService.GetUserByIdAsync(userCtx.UserId);

            if (user.IsBlocked)
            {
                ctx.Response.StatusCode  = StatusCodes.Status403Forbidden;
                ctx.Response.ContentType = "application/json; charset=utf-8";

                var payload = new
                {
                    success = false,
                    message = "دسترسی شما موقتاً غیرفعال است. لطفاً با پشتیبانی تماس بگیرید.",
                    code    = "USER_BLOCKED"
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOpts));
                return;
            }
        }
        catch (Exception ex)
        {
            // اگر هر خطایی در خواندن کاربر رخ داد، اجازه بده پایپ‌لاین ادامه پیدا کنه
            logger.LogError(ex, "BlockedUserGuard: failed to check user blocked state.");
        }

        await next(ctx);
    }

    private static bool IsWhitelisted(string path)
        => _whitelistPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
}

// اکستنشن برای اضافه‌کردن میان‌افزار
public static class BlockedUserGuardExtensions
{
    public static IApplicationBuilder UseBlockedUserGuard(this IApplicationBuilder app)
        => app.UseMiddleware<BlockedUserGuardMiddleware>();
}