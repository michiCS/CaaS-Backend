using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaaS.Api.Extensions;

public static class HttpContextExtensions
{
    public static int GetAuthenticatedTenant(this HttpContext httpContext)
    {
        var identity = httpContext.User as ClaimsPrincipal;
        var value = identity?.FindFirst("tenantId")?.Value;
        if (int.TryParse(value, out int tenantId)) return tenantId;
        return 0;
    }

    public static bool IsAuthorized(this HttpContext httpContext, int tenantId)
    {
        return httpContext.GetAuthenticatedTenant().ToString() == tenantId.ToString();
    }
}
