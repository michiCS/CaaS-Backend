using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class DiscountActionDataDaoTests
{
    private IDiscountActionDataDao discountActionDataDao;

    public DiscountActionDataDaoTests(DataBaseFixture fixture)
    {
        discountActionDataDao = fixture.DiscountActionDataDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await discountActionDataDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Fact]
    public async void FindbyIdAsync_ValidId_ReturnsDiscountActionData()
    {
        var result = await discountActionDataDao.FindByIdAsync(1);
        Assert.Equal(1, result.Id);
        Assert.Equal(10, result.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await discountActionDataDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindByTenantIdAsync_ValidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await discountActionDataDao.FindByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, actionData => Assert.Equal(id, actionData.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindByTenantIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await discountActionDataDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var discountAction = new DiscountActionData { Value = 5, TenantId = 1, ActionType = DiscountActionType.Percentage };
        var result = await discountActionDataDao.InsertAsync(discountAction);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsDiscountActionData()
    {
        var discountAction = new DiscountActionData { Value = 10, TenantId = 2, ActionType = DiscountActionType.Fixed };
        var result = await discountActionDataDao.InsertAndGetAsync(discountAction);
        Assert.True(result.Id == 4 || result.Id == 5); // depends on unit test execution order
        Assert.Equal(10, result.Value);
        Assert.Equal(2, result.TenantId);
        Assert.Equal(DiscountActionType.Fixed, result.ActionType);
    }

    [Fact]
    public async void DeleteAsync_ValidId_ReturnsTrueAndRemovesDiscountActionData()
    {
        var result = await discountActionDataDao.DeleteAsync(3);
        Assert.True(result);
        Assert.Null(await discountActionDataDao.FindByIdAsync(3));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void DeleteAsync_InvalidId_ReturnsFalse(int id)
    {
        var result = await discountActionDataDao.DeleteAsync(id);
        Assert.False(result);
    }

    [Fact]
    public async void UpdateAsync_Existing_ReturnsTrueAndUpdatesRow()
    {
        var toUpdate = new DiscountActionData { Id = 2, Value = 80, ActionType = DiscountActionType.Fixed };
        var result = await discountActionDataDao.UpdateAsync(toUpdate);
        Assert.True(result);

        var updated = await discountActionDataDao.FindByIdAsync(2);
        Assert.Equal(2, updated.Id);
        Assert.Equal(80, updated.Value);
        Assert.Equal(DiscountActionType.Fixed, updated.ActionType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void UpdateAsync_Nonexisting_ReturnsFalse(int id)
    {
        var toUpdate = new DiscountActionData { Id = id, Value = 80, ActionType = DiscountActionType.Fixed };
        var result = await discountActionDataDao.UpdateAsync(toUpdate);
        Assert.False(result);
    }
}
