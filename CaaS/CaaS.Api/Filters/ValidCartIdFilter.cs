using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidCartIdFilter : ParameterizedFilter
{
    private readonly ICartDao cartDao;

    public ValidCartIdFilter(IDaoProvider daoProvider, string propertyString = "cartId") : base(propertyString)
    {
        cartDao = daoProvider.CartDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cartId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (cartId == 0)
        {
            throw new ArgumentException(nameof(cartId));
        }

        var cart = await cartDao.FindByIdAsync(cartId);

        if (cart is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidCartId(cartId));
        }
        else
        {
            context.RouteData.Values.Add("cart", cart);
            await next();
        }
    }
}
    