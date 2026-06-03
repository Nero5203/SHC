using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using ports.DrivenPorts.Purchases;
using ports.DrivingPorts.Purchases;

namespace application.UseCases.Purchases
{
    public class CreatePurchaseUseCase : ICreatePurchaseUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public CreatePurchaseUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Purchase> ExecuteAsync(
            Guid userId,
            Guid subscriptionId,
            decimal amount,
            string currency)
        {
            var purchase = new Purchase
            {
                PurchaseId = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscriptionId,
                Amount = amount,
                Currency = currency,
                PurchasedAt = DateTime.UtcNow,
                Status = PurchaseStatus.Pending
            };

            await _purchaseRepository.CreateAsync(purchase);

            return purchase;
        }
    }
}
