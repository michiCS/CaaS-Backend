using CaaS.Api.Extensions;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class SetCartIdFilter : ParameterizedFilter
{
    private readonly ICartProductDao cartProductDao;

    public SetCartIdFilter(IDaoProvider daoProvider, string propertyString) : base(propertyString)
    {
        cartProductDao = daoProvider.CartProductDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cartProductId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if(cartProductId == 0)
        {
            throw new ArgumentException(nameof(cartProductId));
        }

        var cartId = (await cartProductDao.FindByIdAsync(cartProductId))!.Id;

        context.RouteData.Values.Add("cartId", cartId);

        await next();
    }
}
