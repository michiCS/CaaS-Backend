using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class CartDaoTests
{
    private ICartDao cartDao;

    public CartDaoTests(DataBaseFixture fixture)
    {
        cartDao = fixture.CartDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await cartDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public async void FindbyIdAsync_ValidId_ReturnsCart(int id, int expectedTenantId)
    {
        var result = await cartDao.FindByIdAsync(id);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedTenantId, result.TenantId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await cartDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var cart = new Cart(0, DateTime.UtcNow.Date, 2);
        var result = await cartDao.InsertAsync(cart);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsCart()
    {
        var cart = new Cart(0, DateTime.UtcNow.Date, 3);
        var result = await cartDao.InsertAndGetAsync(cart);
        Assert.True(result.Id == 4 || result.Id == 5); // depends on unit test execution order
        Assert.Equal(3, result.TenantId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindbyTenantIdAsync_ValidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await cartDao.FindByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, cart => Assert.Equal(id, cart.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyTenantIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await cartDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }
}
