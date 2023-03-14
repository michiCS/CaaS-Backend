using CaaS.Core.Dal.Interface;
using Microsoft.AspNetCore.Mvc;
using CaaS.Api.Dtos;
using CaaS.Core.Logic.OrderProcessing;
using CaaS.Api.Dtos.OrderDtos;
using CaaS.Api.Dtos.CartDtos;
using CaaS.Core.Dal.Domain;
using CaaS.Api.Conventions;
using CaaS.Api.Filters;
using Microsoft.AspNetCore.Authorization;

namespace CaaS.Api.Controllers;

[Route("api/orders")]
[AllowAnonymous]
[ApiController]
[ApiConventionType(typeof(OrdersControllerConventions))]
public class OrdersController : Controller
{
    private readonly IOrderDao orderDao;
    private readonly ICartDao cartDao;
    private readonly ICustomerDao customerDao;

    private readonly DtoCreator dtoCreator;
    private readonly IOrderProcessingLogic orderProcessingLogic;

    public OrdersController(IDaoProvider daoProvider, DtoCreator dtoCreator, IOrderProcessingLogic orderProcessingLogic)
    {
        orderDao = daoProvider.OrderDao;
        cartDao = daoProvider.CartDao;
        customerDao = daoProvider.CustomerDao;
        this.dtoCreator = dtoCreator;
        this.orderProcessingLogic = orderProcessingLogic;
    }

    [TypeFilter(typeof(ValidOrderIdFilter))]
    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderById([FromRoute] int orderId)
    {
        var order = (Order)RouteData.Values["order"]!;
        return await dtoCreator.CreateOrderDto(order);
    }

    [TypeFilter(typeof(ValidCartIdFilter), Order = 1, Arguments = new object[] { "orderDto.CartId" })]
    [TypeFilter(typeof(NoOrderExistsFilter), Order = 2, Arguments = new object[] { "orderDto.CartId" })]
    [TypeFilter(typeof(ValidCustomerIdFilter), Order = 3, Arguments = new object[] { "orderDto.CustomerId" })]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderDtoForCreation orderDto)
    {
        var cart = (Cart)RouteData.Values["cart"]!;
        var customer = (Customer)RouteData.Values["customer"]!;

        var order = await orderProcessingLogic.ProcessOrderAsync(cart, customer);

        if (order is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetOrderById),
            routeValues: new { orderId = order.Id },
            value: await dtoCreator.CreateOrderDto(order));
    }
}
