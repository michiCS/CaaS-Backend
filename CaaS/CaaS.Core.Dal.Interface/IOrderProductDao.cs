using CaaS.Core.Dal.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Dal.Interface;

public interface IOrderProductDao
{
    Task<IEnumerable<OrderProduct>> FindAllAsync();
    Task<OrderProduct?> FindByIdAsync(int id);
    Task<bool> InsertAsync(OrderProduct orderProduct);
    Task<IEnumerable<OrderProduct>> FindByOrderIdAsync(int id);
}
