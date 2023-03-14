using CaaS.Core.Dal.Interface;
using CaaS.Core;
using Microsoft.AspNetCore.Mvc;
using CaaS.Api.Dtos;
using CaaS.Api.Dtos.CustomerDtos;
using CaaS.Core.Dal.Domain;
using CaaS.Api.Conventions;
using CaaS.Api.Filters;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace CaaS.Api.Controllers;

[Route("api/customers")]
[AllowAnonymous]
[ApiController]
[ApiConventionType(typeof(CustomersControllerConventions))]
public class CustomersController : Controller
{
    private readonly ICustomerDao customerDao;
    private readonly IMapper mapper;

    public CustomersController(IDaoProvider daoProvider, IMapper mapper)
    {
        customerDao = daoProvider.CustomerDao;
        this.mapper = mapper;
    }

    [TypeFilter(typeof(ValidCustomerIdFilter))]
    [HttpGet("{customerId}")]
    public ActionResult<CustomerDto> GetCustomerById([FromRoute] int customerId)
    {
        var customer = (Customer)RouteData.Values["customer"]!;
        return mapper.Map<CustomerDto>(customer);
    }

    [TypeFilter(typeof(ValidTenantIdFilter), Order = 1)]
    [TypeFilter(typeof(ValidEmailFilter), Order = 2)]
    [HttpGet("tenant/{tenantId}/email/{email}")]
    public ActionResult<CustomerDto> GetCustomerByEmail([FromRoute] string email, [FromRoute] int tenantId)
    {
        var customer = (Customer)RouteData.Values["customer"]!;
        return mapper.Map<CustomerDto>(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerDtoForCreation customerDto)
    {
        var customer = await customerDao.FindByEmailAndTenantIdAsync(customerDto.Email, customerDto.TenantId);
        if (customer is not null)
        {
            return Conflict();
        }

        var insertedCustomer = await customerDao.InsertAndGetAsync(mapper.Map<Customer>(customerDto));

        if(insertedCustomer is null)
        {
            return Problem();
        }

        return CreatedAtAction(
            actionName: nameof(GetCustomerById),
            routeValues: new { customerId = insertedCustomer.Id },
            value: mapper.Map<CustomerDto>(insertedCustomer));
    }
}
