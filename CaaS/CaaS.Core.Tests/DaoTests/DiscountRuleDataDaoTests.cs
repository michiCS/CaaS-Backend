using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class DiscountRuleDataDaoTests
{
    private IDiscountRuleDataDao discountRuleDataDao;

    public DiscountRuleDataDaoTests(DataBaseFixture fixture)
    {
        discountRuleDataDao = fixture.DiscountRuleDataDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await discountRuleDataDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 2, 2)]
    public async void FindbyIdAsync_ValidId_ReturnsDiscountRuleData(int id, int expectedTenantId, int expectedActionId)
    {
        var result = await discountRuleDataDao.FindByIdAsync(id);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedTenantId, result.TenantId);
        Assert.Equal(expectedActionId, result.ActionId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await discountRuleDataDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void FindbyProductIdAsync_ValidId_ReturnsDiscountRuleData()
    {
        var result = await discountRuleDataDao.FindByProductIdAsync(1);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.ActionId);
        Assert.Equal(1, result.TenantId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyProductIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await discountRuleDataDao.FindByProductIdAsync(id);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindbyTenantIdAsync_ValidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await discountRuleDataDao.FindByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, discountRuleData => Assert.Equal(id, discountRuleData.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyTenantIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await discountRuleDataDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindbyActionIdAsync_ValidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await discountRuleDataDao.FindByActionIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, discountRuleData => Assert.Equal(id, discountRuleData.ActionId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyActionIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await discountRuleDataDao.FindByActionIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var discountRule = new DiscountRuleData { TenantId = 1, ActionId = 2, ProductId = 2 };
        var result = await discountRuleDataDao.InsertAsync(discountRule);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsDiscountRuleData()
    {
        var discountRule = new DiscountRuleData { TenantId = 1, ActionId = 2, ProductId = 2 };
        var result = await discountRuleDataDao.InsertAndGetAsync(discountRule);
        Assert.True(result.Id == 5 || result.Id == 6); // depends on unit test execution order
        Assert.Equal(1, result.TenantId);
        Assert.Equal(2, result.ActionId);
        Assert.Equal(2, result.ProductId);
    }

    [Fact]
    public async void DeleteAsync_ValidId_ReturnsTrueAndRemovesDiscountRuleData()
    {
        var result = await discountRuleDataDao.DeleteAsync(4);
        Assert.True(result);
        Assert.Null(await discountRuleDataDao.FindByIdAsync(4));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void DeleteAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await discountRuleDataDao.DeleteAsync(id);
        Assert.False(result);
    }

    [Fact]
    public async void UpdateAsync_Existing_ReturnsTrueAndUpdatesRow()
    {
        var toUpdate = new DiscountRuleData { Id = 3, TenantId = 1, ActionId = 2, ProductId = 4, MinQuantity = 20 };
        var result = await discountRuleDataDao.UpdateAsync(toUpdate);
        Assert.True(result);

        var updated = await discountRuleDataDao.FindByIdAsync(3);
        Assert.Equal(2, updated.ActionId);
        Assert.Equal(4, updated.ProductId);
        Assert.Equal(20, updated.MinQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void UpdateAsync_Nonexisting_ReturnsFalse(int id)
    {
        var toUpdate = new DiscountRuleData { Id = id, TenantId = 1, ActionId = 2, ProductId = 4, MinQuantity = 20 };
        var result = await discountRuleDataDao.UpdateAsync(toUpdate);
        Assert.False(result);
    }
}
