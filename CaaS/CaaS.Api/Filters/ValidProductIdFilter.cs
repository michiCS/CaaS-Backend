using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidProductIdFilter : ParameterizedFilter
{
    private readonly IProductDao productDao;

    public ValidProductIdFilter(IDaoProvider daoProvider, string propertyString = "productId") : base(propertyString)
    {
        productDao = daoProvider.ProductDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var productId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (productId == 0)
        {
            throw new ArgumentException(nameof(productId));
        }

        var product = await productDao.FindByIdAsync(productId);

        if (product is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidProductId(productId));
        }
        else
        {
            context.RouteData.Values.Add("product", product);
            await next();
        }
    }
}
    