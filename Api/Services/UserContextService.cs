using Application.Extensiones;
using Domain.Contract;

namespace Api.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    public long UserId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return 0;

            return httpContext.User.GetLongUserId();
        } 
    }
}