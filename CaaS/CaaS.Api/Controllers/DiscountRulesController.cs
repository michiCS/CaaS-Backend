using AutoMapper;
using CaaS.Api.Conventions;
using CaaS.Api.Dtos.DiscountDtos;
using CaaS.Api.Extensions;
using CaaS.Api.Filters;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CaaS.Api.Controllers;

[Route("api/discountrules")]
[Authorize]
[ApiController]
[ApiConventionType(typeof(DiscountRulesControllerConventions))]
public class DiscountRulesController : Controller
{
    private readonly IDiscountRuleDataDao discountRuleDataDao;
    private readonly IMapper mapper;

    public DiscountRulesController(IDaoProvider daoProvider, IMapper mapper)
    {
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        this.mapper = mapper;
    }

    [TypeFilter(typeof(ValidDiscountRuleIdFilter))]
    [HttpGet("{discountRuleId}")]
    public DiscountRuleDto GetDiscountRuleById([FromRoute] int discountRuleId)
    {
        var discountRule = (DiscountRuleData)RouteData.Values["discountRule"]!;
        return mapper.Map<DiscountRuleDto>(discountRule);
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("tenant")]
    public async Task<IEnumerable<DiscountRuleDto>> GetDiscountRulesByTenantId()
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var discountRules = await discountRuleDataDao.FindByTenantIdAsync(tenantId);
        return discountRules.Select(dr => mapper.Map<DiscountRuleDto>(dr));
    }

    [TypeFilter(typeof(ValidDiscountActionIdFilter), Arguments = new object[] { "discountRuleDto.ActionId" })]
    [HttpPost]
    public async Task<ActionResult<DiscountRuleDto>> CreateDiscountRule([FromBody] DiscountRuleDtoForCreation discountRuleDto)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();

        var discountAction = (DiscountActionData)RouteData.Values["discountAction"]!;

        if (discountAction.TenantId != tenantId)
        {
            return UnprocessableEntity(StatusInfo.DiscountActionInvalidAssignment(discountAction.Id));
        }

        var discountRule = mapper.Map<DiscountRuleData>(discountRuleDto, opt =>
        {
            opt.Items["tenantId"] = tenantId;
        });

        var insertedDiscountRule = await discountRuleDataDao.InsertAndGetAsync(discountRule);

        if(insertedDiscountRule is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetDiscountRuleById),
            routeValues: new { discountRuleId = insertedDiscountRule.Id },
            value: (mapper.Map<DiscountRuleDto>(insertedDiscountRule)));
    }

    [TypeFilter(typeof(ValidDiscountRuleIdFilter))]
    [HttpDelete("{discountRuleId}")]
    public async Task<ActionResult> DeleteDiscountRule([FromRoute] int discountRuleId)
    {
        var discountRule = (DiscountRuleData)RouteData.Values["discountRule"]!;

        if(!HttpContext.IsAuthorized(discountRule.TenantId))
        {
            return Unauthorized();
        }

        var deleted = await discountRuleDataDao.DeleteAsync(discountRuleId);

        if(!deleted)
        {
            return Problem();
        }

        return NoContent();
    }

    [TypeFilter(typeof(ValidDiscountRuleIdFilter), Arguments = new object[] { "discountRuleDto.Id" })]
    [TypeFilter(typeof(ValidDiscountActionIdFilter), Arguments = new object[] { "discountRuleDto.ActionId" })]
    [HttpPatch]
    public async Task<ActionResult> UpdateDiscountRule([FromBody] DiscountRuleDtoForUpdate discountRuleDto)
    {
        var discountRuleTenantId = ((DiscountRuleData)RouteData.Values["discountRule"]!).TenantId;
        var discountAction = (DiscountActionData)RouteData.Values["discountAction"]!;

        var tenantId = HttpContext.GetAuthenticatedTenant();

        if (!HttpContext.IsAuthorized(discountRuleTenantId))
        {
            return Unauthorized();
        }

        if (discountRuleTenantId != discountAction.TenantId)
        {
            return UnprocessableEntity(StatusInfo.DiscountActionInvalidAssignment(discountAction.Id));
        }

        var updated = await discountRuleDataDao.UpdateAsync(mapper.Map<DiscountRuleData>(discountRuleDto));

        if(!updated)
        {
            return Problem();
        }

        return NoContent();
    }

}
