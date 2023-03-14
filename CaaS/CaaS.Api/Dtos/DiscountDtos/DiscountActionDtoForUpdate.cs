using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.DiscountDtos;

public class DiscountActionDtoForUpdate
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public DiscountActionType ActionType { get; set; }
}
