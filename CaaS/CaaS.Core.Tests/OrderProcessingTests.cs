using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;
using CaaS.Core.Logic.OrderProcessing;
using Moq;

namespace CaaS.Core.Tests;

public class OrderProcessingFixture
{
    private List<Customer> customers;
    private List<Product> products;
    private List<Cart> carts;
    private List<CartProduct> cartProducts;
    private List<DiscountAction> discountActions;
    private List<DiscountRule> discountRules;
    public List<Order> orders;
    public List<OrderProduct> orderProducts;

    public Mock<ICustomerDao> customerDao;
    public Mock<ICartDao> cartDao;
    public Mock<IOrderDao> orderDao;
    public Mock<IOrderProductDao> orderProductDao;

    private Mock<IProductDao> productDao;
    private Mock<ICartProductDao> cartProductDao;
    private Mock<IDiscountActionDataDao> discountActionDao;
    private Mock<IDiscountRuleDataDao> discountRuleDao;

    private int mockOrderId = 1;
    private int mockOrderProductId = 1;

    public Mock<IDaoProvider> mockProvider;

    public OrderProcessingFixture()
    {
        InitLists();
        InitMockDaos();

        mockProvider = new Mock<IDaoProvider>();
        mockProvider.Setup(x => x.CustomerDao).Returns(customerDao.Object);
        mockProvider.Setup(x => x.CartDao).Returns(cartDao.Object);
        mockProvider.Setup(x => x.CartProductDao).Returns(cartProductDao.Object);
        mockProvider.Setup(x => x.ProductDao).Returns(productDao.Object);
        mockProvider.Setup(x => x.DiscountActionDataDao).Returns(discountActionDao.Object);
        mockProvider.Setup(x => x.DiscountRuleDataDao).Returns(discountRuleDao.Object);
        mockProvider.Setup(x => x.OrderDao).Returns(orderDao.Object);
        mockProvider.Setup(x => x.OrderProductDao).Returns(orderProductDao.Object);
    }

    private void InitLists()
    {
        customers = new List<Customer>
        {
            new Customer(1, "name1", "email1", 1),
            new Customer(2, "name2", "email2", 2),
        };

        products = new List<Product>
        {
            new Product(1, "prod1", "desc", "link", "url", 0.99m, false, 1),
            new Product(2, "prod2", "desc", "link", "url", 5.49m, false, 1),
            new Product(3, "prod3", "desc", "link", "url", 10m, false, 1),
            new Product(4, "prod4", "desc", "link", "url", 25m, false, 1),
            new Product(5, "prod5", "desc", "link", "url", 17.49m, false, 1),
            new Product(6, "prod6", "desc", "link", "url", 22.35m, false, 1),
            new Product(13, "prod13", "desc", "link", "url", 1.35m, true, 1),

            new Product(7, "prod7", "desc", "link", "url", 50.32m, false, 2),
            new Product(8, "prod8", "desc", "link", "url", 11m, false, 2),
            new Product(9, "prod9", "desc", "link", "url", 99.99m, false, 2),
            new Product(10, "prod10", "desc", "link", "url", 0.50m, false, 2),
            new Product(11, "prod11", "desc", "link", "url", 13m, false, 2),
            new Product(12, "prod12", "desc", "link", "url", 2.29m, false, 2)
        };

        carts = new List<Cart>
        {
            new Cart(1, DateTime.UtcNow.Date, 1),
            new Cart(2, DateTime.UtcNow.Date, 1),
            new Cart(3, DateTime.UtcNow.Date, 2),
            new Cart(4, DateTime.UtcNow.Date, 2),
            new Cart(5, DateTime.UtcNow.Date, 2),
            new Cart(6, DateTime.UtcNow.Date, 1),
        };

        cartProducts = new List<CartProduct>
        {
            new CartProduct(1, 10, 1, 1),     // 10 * 0.99 =  9.90
            new CartProduct(2, 5, 1, 2),      // 5 * 5.49  = 27.45
            new CartProduct(3, 3, 1, 3),      // 3 * 10    = 30
            new CartProduct(4, 2, 1, 4),      // 2 * 25    = 50
                                              //            117.35

            new CartProduct(5, 3, 2, 5),      // 3 * 17.49 =  52.47
            new CartProduct(6, 8, 2, 6),      // 8 * 22.35 = 178.80
                                              //             231.27

            new CartProduct(7, 2, 3, 7),      // 2 * 50.32 = 100.64
            new CartProduct(8, 3, 3, 8),      // 3 * 11    = 33
            new CartProduct(9, 1, 3, 9),      // 1 * 99.99 = 99.99
                                              //            233.63

            new CartProduct(10, 10, 4, 10),   // 10 * 0.5  =  5
            new CartProduct(11, 3, 4, 11),    // 3 * 13    = 39
            new CartProduct(12, 5, 4, 12),    // 5 * 2.29  = 11.45
                                              //             55.45

            new CartProduct(13, 1, 6, 13)
        };

        discountActions = new List<DiscountAction>
        {
            new PercentageDiscountAction(1, 10, 1),
            new PercentageDiscountAction(2, 25, 1),
            new FixedDiscountAction(3, 7, 1),

            new FixedDiscountAction(4, 2, 2),
            new PercentageDiscountAction(5, 33, 2),
            new FixedDiscountAction(6, 4, 2)
        };

        discountRules = new List<DiscountRule>
        {
            new ProductQuantityRule(1, 1, 1, 1, 10),
            new ProductRule(2, 1, 2, 6),
            new ProductRule(3, 1, 3, 5),

            new TemporalRule(4, DiscountApplicationType.CartProduct, 2, 4, DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.AddDays(1).Date),
            new CartTotalRule(5, 2, 5, 200),
            new ProductQuantityRule(6, 2, 6, 11, 2)

        };

        orders = new List<Order>();
        orderProducts = new List<OrderProduct>();
    }

    private void InitMockDaos()
    {
        customerDao = new Mock<ICustomerDao>();
        customerDao.Setup(cd => cd.FindByIdAsync(It.IsAny<int>()))
                   .Returns((int id) => Task.FromResult(customers.SingleOrDefault(c => c.Id == id)));

        cartDao = new Mock<ICartDao>();
        cartDao.Setup(cd => cd.FindByIdAsync(It.IsAny<int>()))
               .Returns((int id) => Task.FromResult(carts.SingleOrDefault(c => c.Id == id)));

        cartProductDao = new Mock<ICartProductDao>();
        cartProductDao.Setup(cd => cd.FindByCartIdAsync(It.IsAny<int>()))
                      .Returns((int cartId) => Task.FromResult(cartProducts.Where(c => c.CartId == cartId)));

        productDao = new Mock<IProductDao>();
        productDao.Setup(pd => pd.FindByIdAsync(It.IsAny<int>()))
                  .Returns((int id) => Task.FromResult(products.SingleOrDefault(p => p.Id == id)));

        discountActionDao = new Mock<IDiscountActionDataDao>();
        discountActionDao.Setup(dd => dd.FindByIdAsync(It.IsAny<int>()))
                         .Returns((int id) => Task.FromResult(discountActions.SingleOrDefault(d => d.Id == id).ToDiscountActionData()));

        discountRuleDao = new Mock<IDiscountRuleDataDao>();
        discountRuleDao.Setup(dd => dd.FindByTenantIdAsync(It.IsAny<int>()))
                       .Returns((int tenantId) => Task.FromResult(discountRules
                                                    .Where(d => d.TenantId == tenantId)
                                                    .Select(d => d.ToDiscountRuleData())));

        orderDao = new Mock<IOrderDao>();
        orderDao.Setup(od => od.InsertAndGetAsync(It.IsAny<Order>())).
                 Returns((Order order) =>
                 {
                     order.Id = mockOrderId;
                     orders.Add(order);
                     mockOrderId++;
                     return Task.FromResult(order);
                 });
        orderDao.Setup(od => od.FindByIdAsync(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(orders.SingleOrDefault(o => o.Id == id)));
        orderDao.Setup(od => od.FindByCartIdAsync(It.IsAny<int>()))
                .Returns((int id) => Task.FromResult(orders.SingleOrDefault(o => o.CartId == id)));

        orderDao.Setup(od => od.UpdateSumDiscounts(It.IsAny<int>(), It.IsAny<decimal>()))
                .Callback((int id, decimal sumDiscounts) => orders.Single(o => o.Id == id).SumDiscounts = sumDiscounts);

        orderProductDao = new Mock<IOrderProductDao>();
        orderProductDao.Setup(od => od.InsertAsync(It.IsAny<OrderProduct>()))
                       .Callback((OrderProduct orderProduct) =>
                       {
                           orderProduct.Id = mockOrderProductId;
                           orderProducts.Add(orderProduct);
                           mockOrderProductId++;
                       });
        orderProductDao.Setup(od => od.FindByOrderIdAsync(It.IsAny<int>()))
                       .Returns((int orderId) => Task.FromResult(orderProducts.Where(o => o.OrderId == orderId)));
    }
}

public class OrderProcessingTests : IClassFixture<OrderProcessingFixture>
{
    private OrderProcessingFixture opf;
    private OrderProcessingLogic logic;

    public OrderProcessingTests(OrderProcessingFixture orderProcessingFixture)
    {
        opf = orderProcessingFixture;
        logic = new OrderProcessingLogic(orderProcessingFixture.mockProvider.Object, new DiscountLogic(orderProcessingFixture.mockProvider.Object));
    }

    public static readonly object[][] TestData =
    {
        new object[]
        {
            1,
            1,
            new Order(1, DateTime.UtcNow.Date, 1, 1m, 1, 1),
            new List<OrderProduct>()
            {
                new OrderProduct(1, 1, 1, 0.89m, 0.1m, 10),
                new OrderProduct(2, 2, 1, 5.49m, 0, 5),
                new OrderProduct(3, 3, 1, 10, 0, 3),
                new OrderProduct(4, 4, 1, 25, 0, 2)
            }
        },
        new object[]
        {
            2,
            1,
            new Order(2, DateTime.UtcNow.Date, 2, 65.72m, 1, 1),
            new List<OrderProduct>()
            {
                new OrderProduct(5, 5, 2, 10.49m, 7, 3),
                new OrderProduct(6, 6, 2, 16.76m, 5.59m, 8)
            }
        },
        new object[]
        {
            3,
            2,
            new Order(3, DateTime.UtcNow.Date, 3, 85.14m, 2, 2),
            new List<OrderProduct>()
            {
                new OrderProduct(7, 7, 3, 48.32m, 2, 2),
                new OrderProduct(8, 8, 3, 9, 2, 3),
                new OrderProduct(9, 9, 3, 97.99m, 2, 1)
            }
        },
        new object[]
        {
            4,
            2,
            new Order(4, DateTime.UtcNow.Date, 4, 27, 2, 2),
            new List<OrderProduct>()
            {
                new OrderProduct(10, 10, 4, 0, 0.5m, 10),
                new OrderProduct(11, 11, 4, 9, 4, 3),
                new OrderProduct(12, 12, 4, 0.29m, 2, 5)
            }
        }
    };

    [Theory, MemberData(nameof(TestData))]
    public async void ProcessOrderAsync_Returns_Order(int cartId, int customerId, Order expectedOrder, List<OrderProduct> expectedOrderProducts)
    {
        var cart = await opf.cartDao.Object.FindByIdAsync(cartId);
        var customer = await opf.customerDao.Object.FindByIdAsync(customerId);

        var order = await logic.ProcessOrderAsync(cart, customer);
        Assert.Equal(expectedOrder, order);

        var orderProducts = (await opf.orderProductDao.Object.FindByOrderIdAsync(order.Id)).ToList();
        Assert.True(expectedOrderProducts.All(orderProducts.Contains));
    }


    public static readonly object[][] TestData2 =
    {
        new object[] { 1, 2 },
        new object[] { 1, 2 },
        new object[] { 2, 2 },
        new object[] { 5, 2 }
    };

    [Theory, MemberData(nameof(TestData2))]
    public async void Process_OrderAsync_Returns_Null(int cartId, int customerId)
    {
        var cart = await opf.cartDao.Object.FindByIdAsync(cartId);
        var customer = await opf.customerDao.Object.FindByIdAsync(customerId);

        var order = await logic.ProcessOrderAsync(cart, customer);
        Assert.Null(order);
    }

}
