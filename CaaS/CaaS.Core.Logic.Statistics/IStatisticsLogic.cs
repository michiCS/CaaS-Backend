using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CaaS.Core.Logic.Statistics.StatisticsLogic;

namespace CaaS.Core.Logic.Statistics;

public interface IStatisticsLogic
{
    Task<decimal> AvgRevenueForTimeInterval(int tenantId, DateTime start, DateTime end);
    Task<IEnumerable<DataSample>> RevenueByDateForTimeInterval(int tenantId, DateTime start, DateTime end);
    Task<Tuple<int, int>> NrOpenAndClosedCarts(int tenantId);
    Task<IEnumerable<SoldProduct>> MostSoldProductsForTimeInterval(int tenantId, DateTime start, DateTime end, int count);
}
