using System.Security.Claims;
using Api.Filters;
using Api.Securities;
using Api.Services;
using Data.Context;
using Domain.Contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Api;

/// <summary>
/// Extension methods to register application services.
/// </summary>
public static class ServiceProvider
{
    /// <summary>
    /// Registers the DbContext with the correct connection string
    /// based on the hosting environment.
    /// </summary>
    public static IServiceCollection ApiServiceProvider(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionHandler>();
                options.Filters.Add<ApiResultFilterAttribute>();
                options.Filters.Add<ValidateModelStateAttribute>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = (ActionContext context) =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors.Select(err => err.ErrorMessage).ToArray()
                        );
                    var problemDetails = new ValidationProblemDetails(errors!)
                    {
                        Title = "Validation Error",
                        Status = StatusCodes.Status400BadRequest,
                    };
                    return new BadRequestObjectResult(problemDetails);
                };
            });

        services.AddHttpContextAccessor();

        // Register current user context (reads UserId from JWT)
        services.AddScoped<IUserContextService, UserContextService>();
        
        services.AddDbContext<NodeShopContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var secret = configuration["JwtSettings:Secret"]!;
                var key = new SymmetricSecurityKey(Convert.FromBase64String(secret));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    RoleClaimType = ClaimTypes.Role,
                };
            });
        services.AddSpaStaticFiles(options => options.RootPath = Path.Combine("EasyUi", "dist"));

        // config CORS
        services.AddCors(options =>
        {
            options.AddPolicy(name: "ShopApiCors", policy =>
            {
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.AllowAnyOrigin();
            });
        });

        services.AddHttpClient();

        return services;
    }
}