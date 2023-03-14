using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc;
using CaaS.Api.Dtos;
using CaaS.Api.Dtos.CartProductDtos;
using CaaS.Api.Conventions;
using CaaS.Api.Filters;
using CaaS.Core.Dal.Domain;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace CaaS.Api.Controllers;

[Route("api/cartproducts")]
[AllowAnonymous]
[ApiController]
[ApiConventionType(typeof(CartProductsControllerConventions))]
public class CartProductsController : Controller
{
    private readonly ICartProductDao cartProductDao;
    private readonly ICartDao cartDao;
    private readonly IProductDao productDao;
    private readonly IOrderDao orderDao;
    private readonly IDiscountRuleDataDao discountRuleDataDao;

    private readonly DtoCreator dtoCreator;
    private readonly IMapper mapper;

    public CartProductsController(IDaoProvider daoProvider, DtoCreator dtoCreator, IMapper mapper)
    {
        cartProductDao = daoProvider.CartProductDao;
        cartDao = daoProvider.CartDao;
        productDao = daoProvider.ProductDao;
        orderDao = daoProvider.OrderDao;
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        this.dtoCreator = dtoCreator;
        this.mapper = mapper;
    }

    [TypeFilter(typeof(ValidCartProductIdFilter))]
    [HttpGet("{cartProductId}")]
    public ActionResult<CartProductDtoSimple> GetCartProductById([FromRoute] int cartProductId)
    {
        var cartProduct = (CartProduct) RouteData.Values["cartProduct"]!;
        return mapper.Map<CartProductDtoSimple>(cartProduct);
    }

    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("cart/{cartId}")]
    public async Task<IEnumerable<CartProductDtoSimple>> GetCartProductsByCartId([FromRoute] int cartId)
    {
        var cartProducts = await cartProductDao.FindByCartIdAsync(cartId);
        return cartProducts.Select(c => mapper.Map<CartProductDtoSimple>(c));
    }

    [TypeFilter(typeof(ValidCartIdFilter), Order = 1, Arguments = new object[] { "cartProductDto.CartId" })]
    [TypeFilter(typeof(NoOrderExistsFilter), Order = 2, Arguments = new object[] { "cartProductDto.CartId" })]
    [TypeFilter(typeof(ValidProductIdFilter), Order = 3, Arguments = new object[] { "cartProductDto.ProductId" })]
    [HttpPost]
    public async Task<ActionResult<CartProductDto>> CreateCartProduct([FromBody] CartProductDtoSimple cartProductDto)
    {
        var cart = (Cart)RouteData.Values["cart"]!;
        var product = (Product)RouteData.Values["product"]!;

        if(cart.TenantId != product.TenantId || product.IsDeleted)
        {
            return UnprocessableEntity(StatusInfo.AddToCartNotPossible(cart.Id, product.Id));
        }

        var existingCartProduct = (await cartProductDao.FindByCartIdAsync(cartProductDto.CartId)).SingleOrDefault(c => c.ProductId == cartProductDto.ProductId);
        if (existingCartProduct is not null)
        {
            await cartProductDao.UpdateQuantityAsync(existingCartProduct.Id, existingCartProduct.Quantity + cartProductDto.Quantity);
            return NoContent();
        }

        var insertedCartProduct = await cartProductDao.InsertAndGetAsync(mapper.Map<CartProduct>(cartProductDto));

        if(insertedCartProduct is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetCartProductById),
            routeValues: new { cartProductId = insertedCartProduct.Id },
            value: (mapper.Map<CartProductDtoSimple>(insertedCartProduct)));
    }

    [TypeFilter(typeof(ValidCartProductIdFilter), Order = 1, Arguments = new object[] { "updateQuantityDto.Id" })]
    [TypeFilter(typeof(SetCartIdFilter), Order = 2, Arguments = new object[] { "updateQuantityDto.Id" })]
    [TypeFilter(typeof(NoOrderExistsFilter), Order = 3, Arguments = new object[] { "RouteData" })]
    [HttpPatch]
    public async Task<ActionResult> UpdateQuantity([FromBody] CartProductUpdateQuantityDto updateQuantityDto) 
    {
        var updated = await cartProductDao.UpdateQuantityAsync(updateQuantityDto.Id, updateQuantityDto.Quantity);
        
        if(!updated)
        {
            return Problem();
        }

        return NoContent();
    }

    [TypeFilter(typeof(ValidCartProductIdFilter), Order = 1)]
    [TypeFilter(typeof(SetCartIdFilter), Order = 2, Arguments = new object[] { "cartProductId" })]
    [TypeFilter(typeof(NoOrderExistsFilter), Order = 3, Arguments = new object[] { "RouteData" })]
    [HttpDelete("{cartProductId}")]
    public async Task<ActionResult> DeleteCartProduct([FromRoute] int cartProductId)
    {
        var deleted = await cartProductDao.DeleteAsync(cartProductId);

        if(!deleted)
        {
            return Problem();
        }

        return NoContent();
    }

    [TypeFilter(typeof(ValidCartIdFilter))]
    [TypeFilter(typeof(NoOrderExistsFilter))]
    [HttpDelete("cart/{cartId}")]
    public async Task<ActionResult> DeleteCartProducts([FromRoute] int cartId)
    {
        var deleted = await cartProductDao.DeleteByCartIdAsync(cartId);

        if(!deleted)
        {
            return Problem();
        }

        return NoContent();
    }
}
