using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Authorization;

namespace CaaS.Api;

public class AppKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITenantDao tenantDao;
    public AppKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IDaoProvider daoProvider
        ) : base(options, logger, encoder, clock)
    {
        this.tenantDao = daoProvider.TenantDao;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var endPoint = Context.GetEndpoint();

        if(endPoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            return AuthenticateResult.NoResult();
        }

        if(!Request.Headers.ContainsKey("app-key"))
        {
            return AuthenticateResult.Fail("Not authenticated");
        }

        var appKey = Request.Headers["app-key"].ToString();
        var tenant = await tenantDao.FindByAppKey(appKey);

        if(tenant is null)
        {
            return AuthenticateResult.Fail("Not authenticated");
        }
        else 
        {
            var claims = new[] { new Claim("app-key", appKey), new Claim("tenantId", tenant.Id.ToString()) };
            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}
