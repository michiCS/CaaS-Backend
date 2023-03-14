using CaaS.Core.Dal.Domain;

namespace CaaS.Core.Logic.OrderProcessing;

public interface IOrderProcessingLogic
{
    Task<Order?> ProcessOrderAsync(Cart cart, Customer customer);
}
