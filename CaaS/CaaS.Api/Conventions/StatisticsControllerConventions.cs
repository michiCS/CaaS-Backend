using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Conventions;

#nullable disable
public static class StatisticsControllerConventions
{
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Get(
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] object param)
    {
    }
}
#nullable restore