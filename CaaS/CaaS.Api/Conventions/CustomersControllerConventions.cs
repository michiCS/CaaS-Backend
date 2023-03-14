using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Conventions;


public static class CustomersControllerConventions
{
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Get(
[ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
[ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] object model)
    {
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Create(
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
[ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
    {
    }
}
