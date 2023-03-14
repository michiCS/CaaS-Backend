using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IDiscountRuleDataDao
{
    Task<IEnumerable<DiscountRuleData>> FindAllAsync();
    Task<DiscountRuleData?> FindByIdAsync(int id);
    Task<DiscountRuleData?> FindByProductIdAsync(int id);
    Task<IEnumerable<DiscountRuleData>> FindByTenantIdAsync(int id);
    Task<IEnumerable<DiscountRuleData>> FindByActionIdAsync(int id);
    Task<bool> InsertAsync(DiscountRuleData discountRule);
    Task<DiscountRuleData?> InsertAndGetAsync(DiscountRuleData discountRule);
    Task<bool> UpdateAsync(DiscountRuleData discountRule);
    Task<bool> DeleteAsync(int id);
}
