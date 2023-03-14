using AutoMapper;
using CaaS.Api.Conventions;
using CaaS.Api.Dtos;
using CaaS.Api.Dtos.DiscountDtos;
using CaaS.Api.Extensions;
using CaaS.Api.Filters;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;

namespace CaaS.Api.Controllers;

[Route("api/discountactions")]
[Authorize]
[ApiController]
[ApiConventionType(typeof(DiscountActionsControllerConventions))]
public class DiscountActionsController : Controller
{
    private readonly IDiscountActionDataDao discountActionDataDao;
    private readonly IDiscountRuleDataDao discountRuleDataDao;
    private readonly DtoCreator dtoCreator;
    private readonly IMapper mapper;

    public DiscountActionsController(IDaoProvider daoProvider, DtoCreator dtoCreator, IMapper mapper)
    {
        discountActionDataDao = daoProvider.DiscountActionDataDao;
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        this.dtoCreator = dtoCreator;
        this.mapper = mapper;
    }

    [AllowAnonymous]
    [TypeFilter(typeof(ValidDiscountActionIdFilter))]
    [HttpGet("{discountActionId}")]
    public DiscountActionDto GetDiscountActionById([FromRoute] int discountActionId)
    {
        var discountAction = (DiscountActionData)RouteData.Values["discountAction"]!;
        return mapper.Map<DiscountActionDto>(discountAction);
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("tenant")]
    public async Task<IEnumerable<DiscountActionDto>> GetDiscountActionsByTenantId()
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var discountActions = await discountActionDataDao.FindByTenantIdAsync(tenantId);
        return discountActions.Select(da => mapper.Map<DiscountActionDto>(da));
    }

    [HttpPost]
    public async Task<ActionResult<DiscountActionDto>> CreateDiscountAction([FromBody] DiscountActionDtoForCreation discountActionDto)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();

        var discountAction = mapper.Map<DiscountActionData>(discountActionDto, opt =>
        {
            opt.Items["tenantId"] = tenantId;
        });

        var insertedDiscountAction = await discountActionDataDao.InsertAndGetAsync(discountAction);

        if(insertedDiscountAction is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetDiscountActionById),
            routeValues: new { discountActionId = insertedDiscountAction.Id },
            value: (mapper.Map<DiscountActionDto>(insertedDiscountAction)));
    }

    [TypeFilter(typeof(ValidDiscountActionIdFilter))]
    [HttpDelete("{discountActionId}")]
    public async Task<ActionResult> DeleteDiscountAction([FromRoute] int discountActionId)
    {
        var associatedRules = await discountRuleDataDao.FindByActionIdAsync(discountActionId);

        if(!associatedRules.IsNullOrEmpty())
        {
            return Conflict(StatusInfo.DiscountActionDeleteNotPossible(discountActionId));
        }

        var discountAction = (DiscountActionData)RouteData.Values["discountAction"]!;

        if (!HttpContext.IsAuthorized(discountAction.TenantId))
        {
            return Unauthorized();
        }

        var deleted = await discountActionDataDao.DeleteAsync(discountActionId);

        if (!deleted)
        {
            return Problem();
        }

        return NoContent();
    }

    [TypeFilter(typeof(ValidDiscountActionIdFilter), Arguments = new object[] { "discountActionDto.Id" })]
    [HttpPatch]
    public async Task<ActionResult> UpdateDiscountAction([FromBody] DiscountActionDtoForUpdate discountActionDto)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();

        if (!HttpContext.IsAuthorized(tenantId))
        {
            return Unauthorized();
        }

        var discountAction = mapper.Map<DiscountActionData>(discountActionDto, opt =>
        {
            opt.Items["tenantId"] = tenantId;
        });

        var updated = await discountActionDataDao.UpdateAsync(discountAction);

        if(!updated)
        {
            return Problem();
        }

        return NoContent();
    }
}
