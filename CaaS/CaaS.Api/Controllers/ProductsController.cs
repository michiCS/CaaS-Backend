using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc;
using CaaS.Core.Dal.Domain;
using Microsoft.AspNetCore.Authorization;
using CaaS.Api.Dtos.ProductDtos;
using CaaS.Api.Conventions;
using CaaS.Api.Filters;
using AutoMapper;
using CaaS.Api.Extensions;

namespace CaaS.Api.Controllers;

[Route("api/products")]
[Authorize]
[ApiController]
[ApiConventionType(typeof(ProductsControllerConventions))]
public class ProductsController : Controller
{
    private readonly IProductDao productDao;
    private readonly ITenantDao tenantDao;
    private readonly IMapper mapper;

    public ProductsController(IDaoProvider daoProvider, IMapper mapper)
    {
        productDao = daoProvider.ProductDao;
        tenantDao = daoProvider.TenantDao;
        this.mapper = mapper;
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProductsOfTenant()
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        return Ok((await productDao.FindByTenantIdAsync(tenantId)).Select(p => mapper.Map<ProductDto>(p)));
    }

    [AllowAnonymous]
    [HttpGet("tenant/{tenantId}/available")]
    public async Task<IEnumerable<ProductDto>> GetAvailableProductsOfTenant([FromRoute] int tenantId)
    {
        return (await productDao.FindAvailableByTenantIdAsync(tenantId)).Select(p => mapper.Map<ProductDto>(p));
    }

    [AllowAnonymous]
    [HttpGet("tenant/{tenantId}/available/count")]
    public async Task<int> GetAvailableProductsCountByTenantId([FromRoute] int tenantId)
    {
        return await productDao.FindRecordCountForPagination(tenantId);
    }

    [AllowAnonymous]
    [HttpGet("tenant/{tenantId}/pagination")]
    public async Task<IEnumerable<ProductDto>> GetProductsPagination([FromRoute] int tenantId, [FromQuery] int limit, [FromQuery] int offset)
    {
        return (await productDao.FindAvailableByTenantIdPaginationAsync(tenantId, limit, offset)).Select(p => mapper.Map<ProductDto>(p));
    }

    [AllowAnonymous]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [TypeFilter(typeof(ValidProductIdFilter))]
    [HttpGet("{productId}")]
    public ActionResult<ProductDto> GetProductById([FromRoute] int productId)
    {
        var product = (Product)RouteData.Values["product"]!;
        return mapper.Map<ProductDto>(product);
    }

    [AllowAnonymous]
    [HttpGet("tenant/{tenantId}/search")]
    public async Task<IEnumerable<ProductDto>> GetProductsBySearchTerm([FromQuery] string term, [FromRoute] int tenantId)
    {
        return (await productDao.FindAvailableBySearchTextAsync(term, tenantId)).Select(p => mapper.Map<ProductDto>(p));
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDtoForCreation productDto)
    {
        var tenantId = HttpContext.GetAuthenticatedTenant();
        var product = mapper.Map<Product>(productDto, opt =>
        {
            opt.Items["tenantId"] = tenantId;
        });

        var insertedProduct = await productDao.InsertAndGetAsync(product);

        if (insertedProduct is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetProductById),
            routeValues: new { productId = insertedProduct.Id },
            value: mapper.Map<ProductDto>(insertedProduct));
    }

    [TypeFilter(typeof(ValidProductIdFilter))]
    [HttpDelete("{productId}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] int productId)
    {
        var product = (Product)RouteData.Values["product"]!;

        if (!HttpContext.IsAuthorized(product.TenantId))
        {
            return Unauthorized();
        }

        var deleted = await productDao.DeleteAsync(productId);

        if (!deleted)
        {
            return Problem();
        }

        return NoContent();
    }

    [TypeFilter(typeof(ValidProductIdFilter), Arguments = new object[] { "productDto.Id" })]
    [HttpPatch]
    public async Task<ActionResult> UpdateProduct([FromBody] ProductDto productDto)
    {
        var product = (Product)RouteData.Values["product"]!;
        var tenantId = HttpContext.GetAuthenticatedTenant();

        if (tenantId != product.TenantId)
        {
            return Unauthorized();
        }

        product = mapper.Map<Product>(productDto, opt =>
        {
            opt.Items["tenantId"] = tenantId;
        });

        var updated = await productDao.UpdateAsync(product);

        if (!updated)
        {
            return Problem();
        }

        return NoContent();
    }
}
