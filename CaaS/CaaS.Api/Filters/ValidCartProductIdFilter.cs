using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidCartProductIdFilter : ParameterizedFilter
{
    private readonly ICartProductDao cartProductDao;

    public ValidCartProductIdFilter(IDaoProvider daoProvider, string propertyString = "cartProductId") : base(propertyString)
    {
        cartProductDao = daoProvider.CartProductDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cartProductId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (cartProductId == 0)
        {
            throw new ArgumentException(nameof(cartProductId));
        }

        var cartProduct = await cartProductDao.FindByIdAsync(cartProductId);

        if (cartProduct is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidCartProductId(cartProductId));
        }
        else
        {
            context.RouteData.Values.Add("cartProduct", cartProduct);
            await next();
        }
    }
}

