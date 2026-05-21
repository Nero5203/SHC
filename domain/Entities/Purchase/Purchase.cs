using Domain.Entities.Purchase.Enums;
using UserEntity = Domain.Entities.User.User;

namespace Domain.Entities.Purchase
{
    public class Purchase
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";

        public DateTime PurchasedAt { get; set; }

        public PurchaseStatus Status { get; set; }
    }
}
