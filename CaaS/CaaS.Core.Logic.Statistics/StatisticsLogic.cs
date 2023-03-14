using CaaS.Core.Dal.Domain;
using CaaS.Core.Dal.Interface;
using CaaS.Core.Logic.Discount;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CaaS.Core.Logic.Statistics;

public partial class StatisticsLogic : IStatisticsLogic
{
    private readonly IOrderDao orderDao;
    private readonly IOrderProductDao orderProductDao;
    private readonly ICartDao cartDao;
    private readonly IProductDao productDao;

    private IDiscountLogic discountLogic;


    public StatisticsLogic(IDaoProvider daoProvider, IDiscountLogic discountLogic)
    {
        cartDao = daoProvider.CartDao;
        orderDao = daoProvider.OrderDao;
        orderProductDao = daoProvider.OrderProductDao;
        productDao = daoProvider.ProductDao;
        this.discountLogic = discountLogic;
    }

    public async Task<decimal> AvgRevenueForTimeInterval(int tenantId, DateTime start, DateTime end)
    {
        var revenues = new List<decimal>();
        var orders = await orderDao.FindByTenantIdAsync(tenantId);
        foreach (DateTime date in EachCalendarDay(start, end))
        {
            var revenue = await CalculateRevenueByDate(orders, date);
            revenues.Add(revenue);
        }

        return Math.Round(revenues.Average(), 2);
    }

    public async Task<IEnumerable<DataSample>> RevenueByDateForTimeInterval(int tenantId, DateTime start, DateTime end)
    {
        var result = new List<DataSample>();
        var orders = await orderDao.FindByTenantIdAsync(tenantId);
        foreach (DateTime date in EachCalendarDay(start, end))
        {
            var revenue = await CalculateRevenueByDate(orders, date);
            result.Add(new DataSample { DateString = date.ToString("yyyy-MM-dd"), Value = revenue });
        }

        return result;
    }

    private async Task<decimal> CalculateRevenueByDate(IEnumerable<Order> orders, DateTime date)
    {
        var dateOrders = orders.Where(o => o.Date.Date == date.Date);
        decimal revenue = 0;
        foreach (var order in dateOrders)
        {
            revenue += (await discountLogic.GetPriceForOrderAsync(order)).Total;
        };
        return revenue;
    }

    private IEnumerable<DateTime> EachCalendarDay(DateTime start, DateTime end)
    {
        for (var date = start.Date; date.Date <= end.Date; date = date.AddDays(1))
        {
            yield return date;
        }
    }

    public async Task<Tuple<int, int>> NrOpenAndClosedCarts(int tenantId)
    {
        int nrOpen = 0;
        int nrClosed = 0;

        var carts = await cartDao.FindByTenantIdAsync(tenantId);
        foreach (var cart in carts)
        {
            var order = await orderDao.FindByCartIdAsync(cart.Id);
            if (order is null)
            {
                nrOpen++;
            }
            else
            {
                nrClosed++;
            }
        }

        return new Tuple<int, int>(nrOpen, nrClosed);
    }

    public async Task<IEnumerable<SoldProduct>> MostSoldProductsForTimeInterval(int tenantId, DateTime start, DateTime end, int count)
    {
        var orders = (await orderDao.FindByTenantIdAsync(tenantId)).Where(o => o.Date >= start && o.Date <= end);

        return orders
                    .Select(async o => await orderProductDao.FindByOrderIdAsync(o.Id))
                    .SelectMany(t => t.Result)
                    .GroupBy(op => op.ProductId)
                    .Select(async a => new SoldProduct
                    {
                        ProductName = (await productDao.FindByIdAsync(a.First().ProductId))!.Name,
                        NrSold = a.Sum(c => c.Quantity)
                    })
                    .Select(a => a.Result)
                    .OrderByDescending(a => a.NrSold)
                    .ThenByDescending(a => a.ProductName)
                    .Take(count);
    }
}
