using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaaS.Core.Logic.Discount.DiscountLogic;

namespace CaaS.Core.Logic.Discount;

public interface IDiscountLogic
{
    Task<decimal> GetDiscountForCartProductAsync(CartProduct cartProduct, IEnumerable<DiscountRule> rules);
    Task<ProductPriceInfo> GetProductPriceInfoAsync(CartProduct cartProduct, IEnumerable<DiscountRule> rules);
    PriceInfo GetPriceInfo(IEnumerable<ProductPriceInfo> priceInfos, IEnumerable<DiscountRule> rules);
    decimal CalculateSumDiscounts(IEnumerable<ProductPriceInfo> priceInfos, IEnumerable<DiscountRule> rules);
    Task<PriceInfo> GetPriceForOrderAsync(Order order);
}
