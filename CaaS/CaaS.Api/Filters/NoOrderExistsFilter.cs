using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using CaaS.Api.Extensions;

namespace CaaS.Api.Filters;

public class NoOrderExistsFilter : ParameterizedFilter
{
    private readonly IOrderDao orderDao;

    public NoOrderExistsFilter(IDaoProvider daoProvider, string propertyString = "cartId") : base(propertyString)
    {
        orderDao = daoProvider.OrderDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int cartId = 0;

        if (propertyString == "RouteData")
        {
            cartId = (int)context.RouteData.Values["cartId"]!;
        }
        else
        {
            cartId = (int)(context.GetValueByString(propertyString) ?? default(int));
        }

        if (cartId == 0)
        {
            throw new ArgumentException(nameof(cartId));
        }

        if ((await orderDao.FindByCartIdAsync(cartId)) is not null)
        {
            context.Result = new ConflictObjectResult(StatusInfo.CartIdProcessed(cartId));
        }

        if (context.Result is null)
        {
            await next();
        }
    }
}
    