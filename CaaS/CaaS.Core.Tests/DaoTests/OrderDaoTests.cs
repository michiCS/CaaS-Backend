using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class OrderDaoTests
{
    private IOrderDao orderDao;

    public OrderDaoTests(DataBaseFixture fixture)
    {
        orderDao = fixture.OrderDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await orderDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void FindbyIdAsync_ValidId_ReturnsCart()
    {
        var result = await orderDao.FindByIdAsync(1);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.CartId);
        Assert.Equal(1, result.CustomerId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await orderDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var order = new Order(0, DateTime.UtcNow.Date, 2, 0, 2, 1);
        var result = await orderDao.InsertAsync(order);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsOrder()
    {
        var order = new Order(0, DateTime.UtcNow.Date, 3, 0, 2, 1);
        var result = await orderDao.InsertAndGetAsync(order);
        Assert.True(result.Id == 2 || result.Id == 3); // depends on unit test execution order
        Assert.Equal(3, result.CartId);
        Assert.Equal(2, result.CustomerId);
        Assert.Equal(1, result.TenantId);
    }

    [Fact]
    public async void FindByCartIdAsync_ValidId_ReturnsOrder()
    {
        var result = await orderDao.FindByCartIdAsync(1);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.CartId);
        Assert.Equal(1, result.CustomerId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindByCartIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await orderDao.FindByCartIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void FindByTenantIdAsync_ValidId_ReturnsNonEmptyEnumerable()
    {
        var result = await orderDao.FindByTenantIdAsync(1);
        Assert.NotEmpty(result);
        Assert.All(result, order => Assert.Equal(1, order.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindByTenantIdAsync_InvalidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await orderDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void UpdateSumDiscounts_ValidId_ReturnsTrueAndUpdatesSumDiscounts()
    {
        var result = await orderDao.UpdateSumDiscounts(1, 50);
        Assert.True(result);

        var order = await orderDao.FindByIdAsync(1);
        Assert.Equal(50, order.SumDiscounts);
    }
}
