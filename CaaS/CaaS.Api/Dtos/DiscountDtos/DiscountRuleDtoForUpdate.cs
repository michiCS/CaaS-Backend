using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.DiscountDtos;

public class DiscountRuleDtoForUpdate
{
    public int Id { get; set; }
    public DiscountApplicationType ApplicationType { get; set; }
    public decimal? MinCartTotal { get; set; }
    public int? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int ActionId { get; set; }
}
