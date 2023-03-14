using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.DiscountDtos;

public class DiscountActionDto
{
    public int Id { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; } = null!;
    public DiscountActionType ActionType { get; set; }
}