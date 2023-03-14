using AutoMapper;
using CaaS.Api.Conventions;
using CaaS.Api.Dtos.TenantDtos;
using CaaS.Api.Extensions;
using CaaS.Api.Filters;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
[ApiConventionType(typeof(TenantsControllerConventions))]
public class TenantsController : Controller
{
    private readonly ITenantDao tenantDao;
    private readonly IMapper mapper;
    private readonly IAppKeyGenerator appKeyGenerator;

    public TenantsController(IDaoProvider daoProvider, IAppKeyGenerator appKeyGenerator, IMapper mapper)
    {
        tenantDao = daoProvider.TenantDao;
        this.mapper = mapper;
        this.appKeyGenerator = appKeyGenerator;
    }

    [TypeFilter(typeof(ValidTenantIdFilter))]
    [HttpGet("{tenantId}")]
    public ActionResult<TenantDto> GetTenantById([FromRoute] int tenantId)
    {
        var tenant = (Tenant)RouteData.Values["tenant"]!;
        return mapper.Map<TenantDto>(tenant);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<TenantDto>> CreateTenant([FromBody] TenantDtoForCreation tenantDto)
    {
        var appKey = appKeyGenerator.GenerateAppKey();

        var insertedTenant = await tenantDao.InsertAndGetAsync(new Tenant(0, tenantDto.Name, appKey));

        if (insertedTenant is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetTenantById),
            routeValues: new { tenantId = insertedTenant.Id },
            value: mapper.Map<TenantDto>(insertedTenant));
    }

    [HttpPatch]
    public async Task<ActionResult<TenantDto>> UpdateTenant([FromBody] TenantDtoForUpdate tenantDto)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var tenant = (await tenantDao.FindByIdAsync(tenantId))!;

        tenant.Name = tenantDto.Name;

        if (tenantDto.RequestNewAppKey)
        {
            tenant.AppKey = appKeyGenerator.GenerateAppKey();
        }

        var updated = await tenantDao.UpdateAsync(tenant);

        if (!updated)
        {
            return Problem();
        }

        return Ok(mapper.Map<TenantDto>((await tenantDao.FindByIdAsync(tenant.Id))!));
    }

}
