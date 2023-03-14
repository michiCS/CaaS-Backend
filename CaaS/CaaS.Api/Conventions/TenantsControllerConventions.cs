using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Conventions;

#nullable disable
public static class TenantsControllerConventions
{
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Get(
[ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] int id)
    {
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Create(
[ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
    {
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Update(
 [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
    {
    }
}
#nullable restore
