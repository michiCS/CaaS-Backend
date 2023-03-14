using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class OrderProductDaoTests
{
    private IOrderProductDao orderProductDao;

    public OrderProductDaoTests(DataBaseFixture fixture)
    {
        orderProductDao = fixture.OrderProductDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await orderProductDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, 1, 1, 1.0)]
    [InlineData(2, 2, 1, 4.0)]
    public async void FindbyIdAsync_ValidId_ReturnsOrderProduct(int id, int exptectedProductId, int expectedOrderId, decimal expectedPrice)
    {
        var result = await orderProductDao.FindByIdAsync(id);
        Assert.Equal(id, result.Id);
        Assert.Equal(exptectedProductId, result.ProductId);
        Assert.Equal(expectedOrderId, result.OrderId);
        Assert.Equal(expectedPrice, result.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await orderProductDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void FindbyOrderIdAsync_ValidOrderId_ReturnsNonEmptyEnumerable()
    {
        var result = await orderProductDao.FindByOrderIdAsync(1);
        Assert.NotEmpty(result);
        Assert.All(result, orderProduct => Assert.Equal(1, orderProduct.OrderId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyOrderIdAsync_InvalidOrderId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await orderProductDao.FindByOrderIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var orderProduct = new OrderProduct(0, 2, 1, 1.0m, 0, 1);
        var result = await orderProductDao.InsertAsync(orderProduct);
        Assert.True(result);
    }
}
