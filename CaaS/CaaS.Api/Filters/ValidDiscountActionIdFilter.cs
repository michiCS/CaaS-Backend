using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CaaS.Api.Extensions;

namespace CaaS.Api.Filters;

public class ValidDiscountActionIdFilter : ParameterizedFilter
{
    private readonly IDiscountActionDataDao discountActionDataDao;

    public ValidDiscountActionIdFilter(IDaoProvider daoProvider, string propertyString = "discountActionId") : base(propertyString)
    {
        discountActionDataDao = daoProvider.DiscountActionDataDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var discountActionId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (discountActionId == 0)
        {
            throw new ArgumentException(nameof(discountActionId));
        }

        var discountAction = await discountActionDataDao.FindByIdAsync(discountActionId);

        if (discountAction is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidDiscountActionId(discountActionId));
        }
        else
        {
            context.RouteData.Values.Add("discountAction", discountAction);
            await next();
        }
    }
}
