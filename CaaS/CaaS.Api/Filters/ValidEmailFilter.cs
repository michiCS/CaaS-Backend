using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Filters;
using CaaS.Api.Extensions;

namespace CaaS.Api.Filters;

public class ValidEmailFilter : ParameterizedFilter
{
    private readonly ICustomerDao customerDao;

    public ValidEmailFilter(IDaoProvider daoProvider, string propertyString = "email") : base(propertyString)
    {
        customerDao = daoProvider.CustomerDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var email = context.GetValueByString(propertyString)?.ToString() ?? default(string);

        if (email.IsNullOrEmpty())
        {
            throw new ArgumentException(nameof(email));
        }

        var tenantId = (int)(context.GetValueByString("tenantId"))!;

        var customer = await customerDao.FindByEmailAndTenantIdAsync(email!, tenantId);

        if (customer is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidCustomerEmail(email!));
        }
        else
        {
            context.RouteData.Values.Add("customer", customer);
            await next();
        }
    }
}
    