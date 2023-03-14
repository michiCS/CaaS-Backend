using AutoMapper;
using CaaS.Api.Conventions;
using CaaS.Api.Dtos;
using CaaS.Api.Dtos.CartDtos;
using CaaS.Api.Filters;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaaS.Api.Controllers;

[Route("api/carts")]
[AllowAnonymous]
[ApiController]
[ApiConventionType(typeof(CartsControllerConventions))]
public class CartsController : Controller
{
    private readonly ICartDao cartDao;
    private readonly ITenantDao tenantDao;
    private readonly IOrderDao orderDao;
    private readonly DtoCreator dtoCreator;
    private readonly IMapper mapper;

    public CartsController(IDaoProvider daoProvider, DtoCreator dtoCreator, IMapper mapper)
    {
        cartDao = daoProvider.CartDao;
        tenantDao = daoProvider.TenantDao;
        orderDao = daoProvider.OrderDao;
        this.dtoCreator = dtoCreator;
        this.mapper = mapper;
    }

    [TypeFilter(typeof(ValidCartIdFilter), Order = 1)]
    [TypeFilter(typeof(NoOrderExistsFilter), Order = 2)]
    [HttpGet("{cartId}/calculated")]
    public async Task<ActionResult<CartDto>> GetCalculatedCartById([FromRoute] int cartId)
    {
        var cart = (Cart)RouteData.Values["cart"]!;
        return await dtoCreator.CreateCartDto(cart);
    }

    [TypeFilter(typeof(ValidCartIdFilter))]
    [HttpGet("{cartId}")]
    public ActionResult<CartDtoSimple> GetCartById([FromRoute] int cartId)
    {
        var cart = (Cart)RouteData.Values["cart"]!;
        return mapper.Map<CartDtoSimple>(cart);
    }

    [TypeFilter(typeof(ValidTenantIdFilter), Arguments = new object[] { "cartDto.TenantId" })]
    [HttpPost]
    public async Task<ActionResult<CartDtoSimple>> CreateCart([FromBody] CartDtoForCreation cartDto)
    {
        var cart = new Cart(0, DateTime.UtcNow.Date, cartDto.TenantId);
        var insertedCart = await cartDao.InsertAndGetAsync(cart);

        if(insertedCart is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetCartById),
            routeValues: new { cartId = insertedCart.Id },
            value: mapper.Map<CartDtoSimple>(insertedCart));
    }
}
