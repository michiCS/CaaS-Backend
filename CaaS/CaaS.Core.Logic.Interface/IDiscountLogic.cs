using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount;
using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Interface;

public interface IDiscountLogic
{
    Task<decimal> FindDiscountForCartProduct(CartProduct cartProduct, IEnumerable<DiscountRule> rules);
    decimal CalculateSumDiscounts(IEnumerable<DiscountedItem> discountedItem, IEnumerable<DiscountRule> discountRules);
}
