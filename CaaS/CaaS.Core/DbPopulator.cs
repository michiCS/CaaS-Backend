using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;
using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using Dal.Common;
using System.Diagnostics;
using System.Globalization;
using System.Transactions;
using CaaS.Core.Logic.Discount;

namespace CaaS.Core;

public class DbPopulator
{
    private readonly ITenantDao tenantDao;
    private readonly IProductDao productDao;
    private readonly ICartDao cartDao;
    private readonly ICartProductDao cartProductDao;
    private readonly IOrderDao orderDao;
    private readonly IOrderProductDao orderProductDao;
    private readonly ICustomerDao customerDao;
    private readonly IDiscountRuleDataDao discountRuleDataDao;
    private readonly IDiscountActionDataDao discountActionDataDao;

    private readonly AdoTemplate adoTemplate;

    private Random rnd;

    private const int startOpenCartId = 2001;
    private const int endOpenCartId = 2010;


    public DbPopulator(IDaoProvider daoProvider)
    {
        tenantDao = daoProvider.TenantDao;
        productDao = daoProvider.ProductDao;
        customerDao = daoProvider.CustomerDao;
        cartDao = daoProvider.CartDao;
        cartProductDao = daoProvider.CartProductDao;
        orderDao = daoProvider.OrderDao;
        orderProductDao = daoProvider.OrderProductDao;
        discountRuleDataDao = daoProvider.DiscountRuleDataDao;
        discountActionDataDao = daoProvider.DiscountActionDataDao;

        adoTemplate = new AdoTemplate(daoProvider.ConnectionFactory);

        rnd = new Random();
    }

    public async Task DeleteAll()
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

    public async Task PopulateAll()
    {
        await PopulateTenants();
        await PopulateProducts();
        await PopulateCustomers();
        await PopulateCarts();
        await PopulateCartProducts();
        await PopulateCartProductsForOpenCarts();
        await PopulateOrders();
        await PopulateOrderProducts();
        await PopulateDiscountActions();
        await PopulateDiscountRules();
    }

    public async Task PopulateTenants()
    {
        Console.WriteLine("==== Populate Tenants ====");

        await tenantDao.InsertAsync(new Tenant(0, "AnimalPhotographyStore", "b22e008f-b1e5-4318-9bf0-8315f3da278c"));
        await tenantDao.InsertAsync(new Tenant(0, "OnlineMovieShop", "43e7d94d-ec35-4a16-b9f2-9d8d1a0f8fcb"));

        (await tenantDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateProducts()
    {
        Console.WriteLine("==== Populate Products ====");

        try
        {
            using (var reader = new StreamReader("Testdata/Products.csv"))
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line is not null && line.Length != 0)
                        {
                            var values = line.Split(";");
                            await productDao.InsertAsync(new Product(0, values[0], values[1], values[2], "https://dummyimage.com/600x400/000/fff", decimal.Parse(values[3], CultureInfo.InvariantCulture), bool.Parse(values[4]), int.Parse(values[5])));
                        }
                    }
                    scope.Complete();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        (await productDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateCustomers()
    {
        Console.WriteLine("==== Populate Customers ====");

        try
        {
            using (var reader = new StreamReader("Testdata/Customers.csv"))
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line is not null && line.Length != 0)
                        {
                            var values = line.Split(";");
                            await customerDao.InsertAsync(new Customer(0, values[0], values[1], int.Parse(values[2])));
                        }
                    }
                    scope.Complete();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

         (await customerDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateCarts()
    {
        Console.WriteLine("==== Populate Carts ====");

        try
        {
            using (var reader = new StreamReader("Testdata/Carts.csv"))
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line is not null && line.Length != 0)
                        {
                            var values = line.Split(";");
                            await cartDao.InsertAsync(new Cart(0, DateTime.UtcNow.Date, int.Parse(values[0])));
                        }
                    }
                    scope.Complete();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        (await cartDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateCartProducts()
    {
        Console.WriteLine("==== Populate Cart Products ====");

        const int nrCartProducts = 6000;

        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = 0; i < nrCartProducts; i++)
                {
                    int cartId = await cartDao.FindRandomId();
                    while (cartId >= startOpenCartId)
                    {
                        cartId = await cartDao.FindRandomId();
                    }
                    await InsertCartProduct(cartId);
                }
                scope.Complete();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        (await cartProductDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));

    }

    public async Task PopulateCartProductsForOpenCarts()
    {
        Console.WriteLine("==== Populate Cart Products for open Carts ====");

        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = startOpenCartId; i <= endOpenCartId; i++)
                {
                    var nrProducts = rnd.Next(0, 10);
                    for(int j = 0; j < nrProducts; j++)
                    {
                        await InsertCartProduct(i);
                    }
                }
                scope.Complete();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        (await cartProductDao.FindAllAsync()).Where(c => c.CartId >= startOpenCartId).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    private async Task InsertCartProduct(int cartId)
    {
        var cart = (await cartDao.FindByIdAsync(cartId));
        if (cart is null)
        {
            throw new InvalidOperationException("Cannot create CartProduct for non existing Cart");
        }
        int productId = await productDao.FindRandomProductIdByTenantId(cart.TenantId);
        int quantity = rnd.Next(1, 10);
        await cartProductDao.InsertAsync(new CartProduct(0, quantity, cartId, productId));
    }

    public async Task PopulateOrders()
    {
        Console.WriteLine("==== Populate Orders ====");

        var carts = (await cartDao.FindAllAsync()).ToList();

        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                carts.Where(c => c.Id < startOpenCartId).ToList().ForEach(async cart =>
                {
                    int customerId = await customerDao.FindRandomCustomerIdByTenantId(cart.TenantId);
                    await orderDao.InsertAsync(new Order(0, DateTime.UtcNow.AddDays(rnd.Next(-100, 100)).Date, cart.Id, 0, customerId, cart.TenantId));

                });
                scope.Complete();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        (await orderDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateOrderProducts()
    {
        Console.WriteLine("==== Populate Order Products ====");

        var cartProducts = (await cartProductDao.FindAllAsync()).ToList();

        try
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                cartProducts.ForEach(async cartProduct =>
                {
                    var order = await orderDao.FindByCartIdAsync(cartProduct.CartId);
                    var product = await productDao.FindByIdAsync(cartProduct.ProductId);
                    if (order is not null && product is not null)
                    {
                        await orderProductDao.InsertAsync(new OrderProduct(0, cartProduct.ProductId, order.Id, product.Price, 0, cartProduct.Quantity));
                    }
                });
                scope.Complete();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

         (await orderProductDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(x + " inserted"));
    }

    public async Task PopulateDiscountActions()
    {
        Console.WriteLine("==== Populate Discount Actions ====");

        await discountActionDataDao.InsertAsync(new PercentageDiscountAction(0, 20, 1).ToDiscountActionData());
        await discountActionDataDao.InsertAsync(new FixedDiscountAction(0, 5, 1).ToDiscountActionData());
        await discountActionDataDao.InsertAsync(new PercentageDiscountAction(0, 10, 1).ToDiscountActionData());
        await discountActionDataDao.InsertAsync(new FixedDiscountAction(0, 15, 1).ToDiscountActionData());
        await discountActionDataDao.InsertAsync(new PercentageDiscountAction(0, 5, 1).ToDiscountActionData());
        await discountActionDataDao.InsertAsync(new FixedDiscountAction(0, 0.5m, 2).ToDiscountActionData());

        (await discountActionDataDao.FindAllAsync()).ToList().ForEach(x =>  Console.WriteLine(DiscountActionHelper.CreateDiscountAction(x)));
    }

    public async Task PopulateDiscountRules()
    {
        Console.WriteLine("==== Populate Discount Rules ====");

        await discountRuleDataDao.InsertAsync(new ProductRule(0, 1, 1, 1).ToDiscountRuleData());
        await discountRuleDataDao.InsertAsync(new TemporalRule(0, DiscountApplicationType.Cart, 2, 2, DateTime.UtcNow.Date.AddDays(-5), DateTime.UtcNow.Date.AddDays(5)).ToDiscountRuleData());

        (await discountRuleDataDao.FindAllAsync()).ToList().ForEach(x => Console.WriteLine(DiscountRuleHelper.CreateDiscountRule(x)));
    }
}