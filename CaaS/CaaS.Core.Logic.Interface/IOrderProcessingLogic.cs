using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Interface;

public interface IOrderProcessingLogic
{
    Task<Order?> ProcessOrder(int cartId, int customerId);
}
