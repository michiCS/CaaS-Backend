using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class ProductDaoTests
{
    private IProductDao productDao;
    public ProductDaoTests(DataBaseFixture fixture)
    {
        productDao = fixture.ProductDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await productDao.FindAllAsync();
        Assert.NotEmpty(result);
    }


    [Theory]
    [InlineData(1, "TestName")]
    [InlineData(2, "TestName2")]
    [InlineData(3, "TestName3")]
    public async void FindbyIdAsync_ValidId_ReturnsProduct(int id, string expectedName)
    {
        var result = await productDao.FindByIdAsync(id);
        Assert.Equal(expectedName, result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await productDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var product = new Product(0, "NewName", "NewDesc", "NewLink", "NewUrl", 1.0m, false, 1);
        var result = await productDao.InsertAsync(product);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsProduct()
    {
        var product = new Product(0, "NewName", "NewDesc", "NewLink", "NewUrl", 1.0m, false, 1);
        var result = await productDao.InsertAndGetAsync(product);
        Assert.True(result.Id == 6 || result.Id == 7); // depends on unit test execution order
        Assert.Equal("NewName", result.Name);
        Assert.Equal("NewDesc", result.Description);
        Assert.Equal("NewLink", result.DownloadLink);
        Assert.Equal("NewUrl", result.ImageUrl);
        Assert.Equal(1, result.Price);
        Assert.False(result.IsDeleted);
        Assert.Equal(1, result.TenantId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindByTenantIdAsync_ValidTenantId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await productDao.FindByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.Equal(id, product.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindByTenantIdAsync_InvalidTenantId_ReturnsEmptyEnumerable(int id)
    {
        var result = await productDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void DeleteAsync_ValidId_ReturnsTrueAndUpdatesIsDeleted()
    {
        var result = await productDao.DeleteAsync(2);
        Assert.True(result);
        Assert.True((await productDao.FindByIdAsync(2)).IsDeleted);
    }

    [Fact]
    public async void DeleteAsync_InvalidId_ReturnsFalse()
    {
        var result = await productDao.DeleteAsync(0);
        Assert.False(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindAvailableByTenantIdAsync_ValidTenantId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await productDao.FindAvailableByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.Equal(id, product.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindAvailableByTenantIdAsync_InvalidTenantId_ReturnsEmptyEnumerable(int id)
    {
        var result = await productDao.FindAvailableByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void UpdateAsync_Existing_ReturnsTrueAndUpdatesRow()
    {
        var toUpdate = new Product(3, "UpdatedName", "UpdatedDesc", "UpdatedLink", "UpdatedUrl", 5.0m, true, 1);
        var result = await productDao.UpdateAsync(toUpdate);
        Assert.True(result);

        var updated = await productDao.FindByIdAsync(3);
        Assert.Equal("UpdatedName", updated.Name);
        Assert.Equal("UpdatedDesc", updated.Description);
        Assert.Equal("UpdatedLink", updated.DownloadLink);
        Assert.Equal("UpdatedUrl", updated.ImageUrl);
        Assert.Equal(5.0m, updated.Price);
        Assert.True(updated.IsDeleted);
    }

    [Fact]
    public async void UpdateAsync_NonExisting_ReturnsFalse()
    {
        var toUpdate = new Product(0, "UpdatedName", "UpdatedDesc", "UpdatedLink", "UpdatedUrl", 3.0m, false, 1);
        var result = await productDao.UpdateAsync(toUpdate);
        Assert.False(result);
    }

    [Theory]
    [InlineData("Test", 1)]
    [InlineData("Desc", 2)]
    public async void FindAvailableBySearchTextAsync_ValidString_ReturnsNonEmptyEnumerable(string search, int tenantId)
    {
        var result = await productDao.FindAvailableBySearchTextAsync(search, tenantId);
        Assert.NotEmpty(result);
        Assert.All(result, product => Assert.True(product.Name.Contains(search) || product.Description.Contains(search)));
    }

    [Theory]
    [InlineData("Key", 1)]
    [InlineData("not contained", 1)]
    [InlineData(" ", 1)]
    public async void FindAvailableBySearchTextAsync_InvalidString_ReturnsEnumerable(string search, int tenantId)
    {
        var result = await productDao.FindAvailableBySearchTextAsync(search, tenantId);
        Assert.Empty(result);
    }
}