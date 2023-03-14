using CaaS.Core.Dal.Interface;
using Dal.Common;
using Microsoft.Extensions.Configuration;

namespace CaaS.Core.Dal.Ado;

public class AdoDaoProvider : IDaoProvider
{
    public IConnectionFactory ConnectionFactory { get; }

    public AdoDaoProvider(string dbName)
    {
        IConfiguration configuration = ConfigurationUtil.GetConfiguration();
        ConnectionFactory = DefaultConnectionFactory.FromConfiguration(configuration, dbName);
    }

    private ITenantDao tenantDao = null!;

    public ITenantDao TenantDao
    {
        get
        {
            if (tenantDao is null)
            {
                tenantDao = new AdoTenantDao(ConnectionFactory);
            }
            return tenantDao;
        }
    }

    private IProductDao productDao = null!;

    public IProductDao ProductDao
    {
        get
        {
            if (productDao is null)
            {
                productDao = new AdoProductDao(ConnectionFactory);
            }
            return productDao;
        }
    }

    private ICustomerDao customerDao = null!;

    public ICustomerDao CustomerDao
    {
        get
        {
            if (customerDao is null)
            {
                customerDao = new AdoCustomerDao(ConnectionFactory);
            }
            return customerDao;
        }
    }

    private ICartDao cartDao = null!;

    public ICartDao CartDao
    {
        get
        {
            if (cartDao is null)
            {
                cartDao = new AdoCartDao(ConnectionFactory);
            }
            return cartDao;
        }
    }

    private ICartProductDao cartProductDao = null!;

    public ICartProductDao CartProductDao
    {
        get
        {
            if (cartProductDao is null)
            {
                cartProductDao = new AdoCartProductDao(ConnectionFactory);
            }
            return cartProductDao;
        }
    }

    private IOrderDao orderDao = null!;

    public IOrderDao OrderDao
    {
        get
        {
            if (orderDao is null)
            {
                orderDao = new AdoOrderDao(ConnectionFactory);
            }
            return orderDao;
        }
    }

    private IOrderProductDao orderProductDao = null!;

    public IOrderProductDao OrderProductDao
    {
        get
        {
            if (orderProductDao is null)
            {
                orderProductDao = new AdoOrderProductDao(ConnectionFactory);
            }
            return orderProductDao;
        }
    }

    private IDiscountActionDataDao discountActionDataDao = null!;

    public IDiscountActionDataDao DiscountActionDataDao
    {
        get
        {
            if (discountActionDataDao is null)
            {
                discountActionDataDao = new AdoDiscountActionDataDao(ConnectionFactory);
            }
            return discountActionDataDao;
        }
    }


    private IDiscountRuleDataDao discountRuleDataDao = null!;

    public IDiscountRuleDataDao DiscountRuleDataDao
    {
        get
        {
            if (discountRuleDataDao is null)
            {
                discountRuleDataDao = new AdoDiscountRuleDataDao(ConnectionFactory);
            }
            return discountRuleDataDao;
        }
    }
}
