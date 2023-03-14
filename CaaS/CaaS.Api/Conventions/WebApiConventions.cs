using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Conventions;

public static class WebApiConventions
{

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Get(
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] object param)
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

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Update(
     [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] int id,

     [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.Any)] object model)
    {
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Prefix)]
    public static void Delete(
     [ApiConventionNameMatch(ApiConventionNameMatchBehavior.Any)]
    [ApiConventionTypeMatch(ApiConventionTypeMatchBehavior.AssignableFrom)] int id)
    {
    }
}
