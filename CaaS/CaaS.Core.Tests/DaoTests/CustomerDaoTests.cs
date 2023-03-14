using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;

namespace CaaS.Core.Tests.DaoTests;

[Collection("Database collection")]
public class CustomerDaoTests
{
    private ICustomerDao customerDao;

    public CustomerDaoTests(DataBaseFixture fixture)
    {
        customerDao = fixture.CustomerDao;
    }

    [Fact]
    public async void FindAllAsync_ReturnsNonEmptyEnumerable()
    {
        var result = await customerDao.FindAllAsync();
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData(1, "TestName")]
    [InlineData(2, "TestName2")]
    public async void FindbyIdAsync_ValidId_ReturnsCustomer(int id, string expectedName)
    {
        var result = await customerDao.FindByIdAsync(id);
        Assert.NotNull(result);
        Assert.Equal(expectedName, result.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyIdAsync_InvalidId_ReturnsNull(int id)
    {
        var result = await customerDao.FindByIdAsync(id);
        Assert.Null(result);
    }

    [Fact]
    public async void FindByEmailAsync_ValidEmail_ReturnsCustomer()
    {
        var result = await customerDao.FindByEmailAndTenantIdAsync("TestEmail", 1);
        Assert.NotNull(result);
        Assert.Equal("TestName", result.Name);
        Assert.Equal("TestEmail", result.Email);
        Assert.Equal(1, result.TenantId);
    }

    [Theory]
    [InlineData("notexistingemail")]
    [InlineData("")]
    [InlineData("TestEmail3")]
    public async void FindByEmailAsync_InvalidEmail_ReturnsNull(string email)
    {
        var result = await customerDao.FindByEmailAndTenantIdAsync(email, 1);
        Assert.Null(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async void FindbyTenantIdAsync_ValidId_ReturnsNonEmptyEnumerable(int id)
    {
        var result = await customerDao.FindByTenantIdAsync(id);
        Assert.NotEmpty(result);
        Assert.All(result, customer => Assert.Equal(id, customer.TenantId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9999)]
    [InlineData(-5)]
    public async void FindbyTenantIdAsync_InvalidId_ReturnsEmptyEnumerable(int id)
    {
        var result = await customerDao.FindByTenantIdAsync(id);
        Assert.Empty(result);
    }

    [Fact]
    public async void InsertAsync_ReturnsTrue()
    {
        var customer = new Customer(0, "NewCustomer", "NewEmail", 1);
        var result = await customerDao.InsertAsync(customer);
        Assert.True(result);
    }

    [Fact]
    public async void InsertAndGetAsync_ReturnsCustomer()
    {
        var customer = new Customer(0, "NewCustomer", "NewEmail", 1);
        var result = await customerDao.InsertAndGetAsync(customer);
        Assert.True(result.Id == 3 || result.Id == 4); // depends on unit test execution order
        Assert.Equal("NewCustomer", result.Name);
        Assert.Equal("NewEmail", result.Email);
        Assert.Equal(1, result.TenantId);
    }
}