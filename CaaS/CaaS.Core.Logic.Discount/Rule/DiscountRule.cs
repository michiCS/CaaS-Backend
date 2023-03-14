using CaaS.Core.Dal.Domain;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Discount.Rule;

public abstract class DiscountRule
{
    public int Id { get; set; }
    public DiscountApplicationType ApplicationType { get; set; }
    public int TenantId { get; set; }
    public int ActionId { get; set; }

    protected DiscountRule(int id, DiscountApplicationType applicationType, int tenantId, int actionId)
    {
        Id = id;
        ApplicationType = applicationType;
        TenantId = tenantId;
        ActionId = actionId;
    }

    protected DiscountRule(DiscountRuleData data) : this(data.Id, data.ApplicationType, data.TenantId, data.ActionId) { }

    public abstract string ToDescription();

    public bool CanApply()
    {
        return true;
    }

    public virtual DiscountRuleData ToDiscountRuleData()
    {
        return new DiscountRuleData
        {
            Id = Id,
            ApplicationType = ApplicationType,
            TenantId = TenantId,
            ActionId = ActionId
        };
    }

    public override string ToString()
    {
        return GetType().Name + JsonSerializer.Serialize(this);
    }
}
