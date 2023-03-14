using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidCustomerIdFilter : ParameterizedFilter
{
    private readonly ICustomerDao customerDao;

    public ValidCustomerIdFilter(IDaoProvider daoProvider, string propertyString = "customerId") : base(propertyString)
    {
        customerDao = daoProvider.CustomerDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var customerId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (customerId == 0)
        {
            throw new ArgumentException(nameof(customerId));
        }

        var customer = await customerDao.FindByIdAsync(customerId);

        if (customer is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidCustomerId(customerId));
        }
        else
        {
            context.RouteData.Values.Add("customer", customer);
            await next();
        }
    }
}
    