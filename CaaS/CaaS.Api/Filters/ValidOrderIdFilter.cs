using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidOrderIdFilter : ParameterizedFilter
{
    private readonly IOrderDao orderDao;

    public ValidOrderIdFilter(IDaoProvider daoProvider, string propertyString = "orderId") : base(propertyString)
    {
        orderDao = daoProvider.OrderDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var orderId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (orderId == 0)
        {
            throw new ArgumentException(nameof(orderId));
        }

        var order = await orderDao.FindByIdAsync(orderId);

        if (order is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidOrderId(orderId));
        }
        else
        {
            context.RouteData.Values.Add("order", order);
            await next();
        }
    }
}
    