using System.Security.Claims;

namespace Application.Extensiones;

public static class ClaimExtension
{
    public static long GetLongUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.SingleOrDefault(s => s.Type == ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            var userId = Convert.ToInt64(claim.Value);
            if (userId != 0) return userId;
        }

        return 0;
    }
}