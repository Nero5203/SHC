using Domain.Entities.Purchases.Enums;

namespace api.Dto.Purchases
{
    public class UpdatePurchaseStatusDto
    {
        public PurchaseStatus Status { get; set; }
    }
}
