using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidTenantIdFilter : ParameterizedFilter
{
    private readonly ITenantDao tenantDao;

    public ValidTenantIdFilter(IDaoProvider daoProvider, string propertyString = "tenantId") : base(propertyString)
    {
        tenantDao = daoProvider.TenantDao;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var tenantId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (tenantId == 0)
        {
            throw new ArgumentException(nameof(tenantId));
        }

        var tenant = await tenantDao.FindByIdAsync(tenantId);

        if (tenant is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidTenantId(tenantId));
        }
        else
        {
            context.RouteData.Values.Add("tenant", tenant);
            await next();
        }
    }
}
    