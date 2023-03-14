using CaaS.Core.Dal.Ado;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class TenantDaoTests
{
    private ITenantDao tenantDao;

    public TenantDaoTests(DataBaseFixture fixture)
    {
        tenantDao = fixture.TenantDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await tenantDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, "TestName")]
    [InlineData(2, "TestName2")]
    [InlineData(3, "TestName3")]
    public async void FindbyIdAsync_ValidId_ReturnsTenant(int id, string expectedName)
    {
        var result = await tenantDao.FindByIdAsync(id);
        Assert.Equal(expectedName, result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await tenantDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var tenant = new Tenant(0, "NewTenant", "NewKey");
        var result = await tenantDao.InsertAsync(tenant);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsTenant()
    {
        var tenant = new Tenant(0, "NewTenant", "NewKey");
        var result = await tenantDao.InsertAndGetAsync(tenant);
        Assert.True(result.Id == 5 || result.Id == 6); // depends on unit test execution order
        Assert.Equal("NewTenant", result.Name);
        Assert.Equal("NewKey", result.AppKey);
    }

    [Theory]
    [InlineData("TestKey", 1)]
    [InlineData("TestKey2", 2)]
    [InlineData("TestKey3", 3)]
    public async void FindByAppKey_ValidAppKey_ReturnsTenant(string key, int expectedId)
    {
        var result = await tenantDao.FindByAppKey(key);
        Assert.Equal(expectedId, result.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid key")]
    [InlineData("12345")]
    public async void FindByAppKey_InvalidAppKey_ReturnsNull(string key)
    {
        var result = await tenantDao.FindByAppKey(key);
        Assert.Null(result);
    }

    [Fact]
    public async void UpdateAsync_Existing_ReturnsTrueAndUpdatesRow()
    {
        var toUpdate = new Tenant(4, "UpdatedName", "UpdatedKey");
        var result = await tenantDao.UpdateAsync(toUpdate);
        Assert.True(result);

        var updated = await tenantDao.FindByIdAsync(4);
        Assert.Equal("UpdatedName", updated.Name);
        Assert.Equal("UpdatedKey", updated.AppKey);
    }

    [Fact]
    public async void UpdateAsync_NonExisting_ReturnsFalse()
    {
        var toUpdate = new Tenant(0, "UpdatedName", "UpdatedKey");
        var result = await tenantDao.UpdateAsync(toUpdate);
        Assert.False(result);
    }
}