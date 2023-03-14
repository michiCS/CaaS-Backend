using CaaS.Core.Dal.Ado;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using Microsoft.Extensions.Configuration;

namespace CaaS.Core.Tests.DaoTests;

public class DataBaseFixture : IDisposable
{
    public ITenantDao TenantDao { get; }
    public IProductDao ProductDao { get; }
    public ICartDao CartDao { get; }
    public ICartProductDao CartProductDao { get; }
    public IOrderDao OrderDao { get; }
    public IOrderProductDao OrderProductDao { get; }
    public ICustomerDao CustomerDao { get; }
    public IDiscountActionDataDao DiscountActionDataDao { get; }
    public IDiscountRuleDataDao DiscountRuleDataDao { get; }

    private AdoTemplate adoTemplate;
    private AdoDaoProvider daoProvider;

    public DataBaseFixture()
    {
        daoProvider = new AdoDaoProvider("TestDbConnection");
        adoTemplate = new AdoTemplate(daoProvider.ConnectionFactory);

        TenantDao = daoProvider.TenantDao;
        ProductDao = daoProvider.ProductDao;
        CustomerDao = daoProvider.CustomerDao;
        CartDao = daoProvider.CartDao;
        CartProductDao = daoProvider.CartProductDao;
        OrderDao = daoProvider.OrderDao;
        OrderProductDao = daoProvider.OrderProductDao;
        DiscountRuleDataDao = daoProvider.DiscountRuleDataDao;
        DiscountActionDataDao = daoProvider.DiscountActionDataDao;

        SetUpTestData();
    }

    public async void SetUpTestData()
    {
        await TenantDao.InsertAsync(new Tenant(0, "TestName", "TestKey"));
        await TenantDao.InsertAsync(new Tenant(0, "TestName2", "TestKey2"));
        await TenantDao.InsertAsync(new Tenant(0, "TestName3", "TestKey3"));
        await TenantDao.InsertAsync(new Tenant(0, "TestName4", "TestKey4"));

        await ProductDao.InsertAsync(new Product(0, "TestName", "TestDesc", "TestLink", "NewUrl", 1.0m, false, 1));
        await ProductDao.InsertAsync(new Product(0, "TestName2", "TestDesc2", "TestLink2", "NewUrl", 2.0m, false, 1));
        await ProductDao.InsertAsync(new Product(0, "TestName3", "TestDesc3", "TestLink3", "NewUrl", 3.0m, false, 2));
        await ProductDao.InsertAsync(new Product(0, "TestName4", "TestDesc4", "TestLink4", "NewUrl", 4.0m, false, 2));
        await ProductDao.InsertAsync(new Product(0, "TestName5", "TestDesc5", "TestLink5", "NewUrl", 5.0m, false, 1));

        await CustomerDao.InsertAsync(new Customer(0, "TestName", "TestEmail", 1));
        await CustomerDao.InsertAsync(new Customer(0, "TestName2", "TestEmail2", 2));

        await CartDao.InsertAsync(new Cart(0, DateTime.UtcNow.Date, 1));
        await CartDao.InsertAsync(new Cart(0, DateTime.UtcNow.Date, 2));
        await CartDao.InsertAsync(new Cart(0, DateTime.UtcNow.Date, 2));

        await CartProductDao.InsertAsync(new CartProduct(0, 1, 1, 1));
        await CartProductDao.InsertAsync(new CartProduct(0, 1, 1, 1));
        await CartProductDao.InsertAsync(new CartProduct(0, 2, 1, 2));
        await CartProductDao.InsertAsync(new CartProduct(0, 3, 2, 3));
        await CartProductDao.InsertAsync(new CartProduct(0, 4, 2, 4));

        await OrderDao.InsertAsync(new Order(0, DateTime.UtcNow.Date, 1, 0, 1, 1));
        await OrderProductDao.InsertAsync(new OrderProduct(0, 1, 1, 1.0m, 0m, 1));
        await OrderProductDao.InsertAsync(new OrderProduct(0, 2, 1, 4.0m, 0m, 2));

        await DiscountActionDataDao.InsertAsync(new DiscountActionData { Value = 10, TenantId = 1, ActionType = DiscountActionType.Fixed });
        await DiscountActionDataDao.InsertAsync(new DiscountActionData { Value = 20, TenantId = 2, ActionType = DiscountActionType.Fixed });
        await DiscountActionDataDao.InsertAsync(new DiscountActionData { Value = 50, TenantId = 1, ActionType = DiscountActionType.Fixed });

        await DiscountRuleDataDao.InsertAsync(new DiscountRuleData { TenantId = 1, ActionId = 1, ProductId = 1 });
        await DiscountRuleDataDao.InsertAsync(new DiscountRuleData { TenantId = 2, ActionId = 2, MinCartTotal = 200 });
        await DiscountRuleDataDao.InsertAsync(new DiscountRuleData { TenantId = 1, ActionId = 1, ProductId = 5, MinQuantity = 10 });
        await DiscountRuleDataDao.InsertAsync(new DiscountRuleData { TenantId = 2, ActionId = 2, ProductId = 3, MinQuantity = 3 });
    }

    public async void Dispose()
    {
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from discount_rule; alter table discount_rule auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from discount_action; alter table discount_action auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from product; alter table product auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from cart; alter table cart auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from cart_product; alter table cart_product auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from order_product; alter table order_product auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from `order`; alter table `order` auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from customer; alter table customer auto_increment=1");
        await adoTemplate.ExecuteAsync("set foreign_key_checks=0; delete from tenant; alter table tenant auto_increment=1");
    }
}