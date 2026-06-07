using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;
using System.Linq;

namespace application.UseCases.Purchases
{
    public class CreatePurchaseUseCase : ICreatePurchaseUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public CreatePurchaseUseCase(
            IPurchaseRepository purchaseRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _purchaseRepository = purchaseRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Purchase> ExecuteAsync(
            Guid userId,
            Guid subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);

            if (subscription == null)
            {
                throw new InvalidOperationException("Subscription was not found.");
            }

            var belongsToUser = subscription.UserSubscriptions
                .Any(userSubscription => userSubscription.UserId == userId && userSubscription.RemovedAt == null);

            if (!belongsToUser)
            {
                throw new InvalidOperationException("Subscription does not belong to the specified user.");
            }

            if (subscription.SubscriptionPlan == null)
            {
                throw new InvalidOperationException("Subscription plan was not found for this subscription.");
            }

            var existingPendingPurchase = await _purchaseRepository.GetPendingBySubscriptionIdAsync(subscriptionId);

            if (existingPendingPurchase != null)
            {
                return existingPendingPurchase;
            }

            var purchase = new Purchase
            {
                PurchaseId = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscriptionId,
                Amount = subscription.SubscriptionPlan.Price,
                Currency = subscription.SubscriptionPlan.Currency,
                PurchasedAt = DateTime.UtcNow,
                Status = PurchaseStatus.Pending
            };

            await _purchaseRepository.CreateAsync(purchase);

            return purchase;
        }
    }
}
