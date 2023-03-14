using Dal.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IDaoProvider
{
    ITenantDao TenantDao { get; }
    IProductDao ProductDao { get; }
    ICustomerDao CustomerDao { get; }
    ICartDao CartDao { get; }
    ICartProductDao CartProductDao { get; }
    IOrderDao OrderDao { get; }
    IOrderProductDao OrderProductDao { get; }
    IDiscountActionDataDao DiscountActionDataDao { get; }
    IDiscountRuleDataDao DiscountRuleDataDao { get; }
    IConnectionFactory ConnectionFactory{ get;  }
}
