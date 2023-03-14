using AutoMapper;
using CaaS.Api.Conventions;
using CaaS.Api.Dtos.StatisticsDtos;
using CaaS.Api.Extensions;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers;

[Route("api/statistics")]
[Authorize]
[ApiController]
[ApiConventionType(typeof(StatisticsControllerConventions))]
public class StatisticsController : Controller
{
    private IStatisticsLogic statisticsLogic;
    private readonly IMapper mapper;

    public StatisticsController(IStatisticsLogic statisticsLogic, IMapper mapper)
    {
        this.statisticsLogic = statisticsLogic;
        this.mapper = mapper;
    }

    [HttpGet("avg-revenue")]
    public async Task<ActionResult<decimal>> GetAvgRevenueForTimeInterval([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        return await statisticsLogic.AvgRevenueForTimeInterval(tenantId, start, end);
    }
 
    [HttpGet("revenue-by-date")]
    public async Task<ActionResult<IEnumerable<DataSampleDto>>> GetRevenueByDateForTimeInterval([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var result = await statisticsLogic.RevenueByDateForTimeInterval(tenantId, start, end);
        return Ok(result.Select(d => mapper.Map<DataSampleDto>(d)));
    }

    [HttpGet("nr-open-closed-carts")]
    public async Task<ActionResult<OpenClosedCartsDto>> GetNrOpenAndClosedCarts()
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var result = await statisticsLogic.NrOpenAndClosedCarts(tenantId);
        return mapper.Map<OpenClosedCartsDto>(result);
    }

    [HttpGet("most-sold-products")]
    public async Task<ActionResult<IEnumerable<SoldProductDto>>> GetMostSoldProductsForYear([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int count)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var result = await statisticsLogic.MostSoldProductsForTimeInterval(tenantId, start, end, count);
        return Ok(result.Select(sp => mapper.Map<SoldProductDto>(sp)));
    }
}
