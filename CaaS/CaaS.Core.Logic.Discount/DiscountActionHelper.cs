using CaaS.Core.Dal.Domain;
using CaaS.Core.Logic.Discount.Action;
using CaaS.Core.Logic.Discount.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Discount;

public static class DiscountActionHelper
{
    public static DiscountAction? CreateDiscountAction(DiscountActionData data)
    {
        if (FixedDiscountAction.CanCreate(data))
        {
            return new FixedDiscountAction(data);
        }

        if (PercentageDiscountAction.CanCreate(data))
        {
            return new PercentageDiscountAction(data);
        }

        return null;
    }
}
