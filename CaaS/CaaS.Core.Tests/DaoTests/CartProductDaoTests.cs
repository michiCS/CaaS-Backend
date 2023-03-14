using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class CartProductDaoTests
{
    private ICartProductDao cartProductDao;

    public CartProductDaoTests(DataBaseFixture fixture)
    {
        cartProductDao = fixture.CartProductDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await cartProductDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void FindbyIdAsync_ValidId_ReturnsCartProduct()
    {
        var result = await cartProductDao.FindByIdAsync(1);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.CartId);
        Assert.Equal(1, result.Quantity);
        Assert.Equal(1, result.ProductId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await cartProductDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var cartProduct = new CartProduct(0, 1, 1, 1);
        var result = await cartProductDao.InsertAsync(cartProduct);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsCartProduct()
    {
        var cartProduct = new CartProduct(0, 1, 1, 1);
        var result = await cartProductDao.InsertAndGetAsync(cartProduct);
        Assert.True(result.Id == 6 || result.Id == 7); // depends on unit test execution order
        Assert.Equal(1, result.Quantity);
        Assert.Equal(1, result.CartId);
        Assert.Equal(1, result.ProductId);
    }

    [Fact]
    public async void UpdateQuantityAsync_ReturnsTrueAndUpdatesQuantity()
    {
        var result = await cartProductDao.UpdateQuantityAsync(3, 5);
        Assert.True(result);

        var cartProduct = await cartProductDao.FindByIdAsync(3);
        Assert.Equal(5, cartProduct.Quantity);
    }

    [Fact]
    public async void DeleteAsync_ValidId_ReturnsTrueAndDeletesCartProduct()
    {
        var result = await cartProductDao.DeleteAsync(2);
        Assert.True(result);
        Assert.Null(await cartProductDao.FindByIdAsync(2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void DeleteAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await cartProductDao.DeleteAsync(id);
        Assert.False(result);
    }

    [Fact]
    public async void FindbyCartIdAsync_ValidId_ReturnsNonEmptyEnumerable()
    {
        var result = await cartProductDao.FindByCartIdAsync(1);
        Assert.NotEmpty(result);
        Assert.All(result, cartProduct => Assert.Equal(1, cartProduct.CartId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyCartIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await cartProductDao.FindByCartIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void DeleteByCartIdAsync_ValidId_ReturnsTrueAndDeletesCartProducts()
    {
        var result = await cartProductDao.DeleteByCartIdAsync(2);
        Assert.True(result);
        Assert.Empty(await cartProductDao.FindByCartIdAsync(2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void DeleteByCartIdAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await cartProductDao.DeleteByCartIdAsync(id);
        Assert.False(result);
    }
}
