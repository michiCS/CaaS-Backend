using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.DiscountDtos;

public class DiscountRuleDto
{
    public int Id { get; set; }
    public DiscountApplicationType ApplicationType { get; set; }
    public decimal? MinCartTotal { get; set; }
    public int? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public string? DateFrom { get; set; }
    public string? DateTo { get; set; }
    public string Description { get; set; } = null!;
    public int ActionId { get; set; }
}