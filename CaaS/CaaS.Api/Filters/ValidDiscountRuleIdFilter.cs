using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CaaS.Api.Filters;

public class ValidDiscountRuleIdFilter : ParameterizedFilter
{
    private readonly IDiscountRuleDataDao discountRuleDataDao;

    public ValidDiscountRuleIdFilter(IDaoProvider daoProvider, string propertyString = "discountRuleId") : base(propertyString)
    {
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var discountRuleId = (int)(context.GetValueByString(propertyString) ?? default(int));

        if (discountRuleId == 0)
        {
            throw new ArgumentException(nameof(discountRuleId));
        }

        var discountRule = await discountRuleDataDao.FindByIdAsync(discountRuleId);

        if (discountRule is null)
        {
            context.Result = new NotFoundObjectResult(StatusInfo.InvalidDiscountRuleId(discountRuleId));
        }
        else
        {
            context.RouteData.Values.Add("discountRule", discountRule);
            await next();
        }
    }
}
