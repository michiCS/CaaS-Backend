using CaaS.Core.Dal.Domain;

namespace CaaS.Api.Dtos.DiscountDtos
{
    public class DiscountActionDtoForCreation
    {
        public decimal Value { get; set; }
        public DiscountActionType ActionType { get; set; }
    }
}
