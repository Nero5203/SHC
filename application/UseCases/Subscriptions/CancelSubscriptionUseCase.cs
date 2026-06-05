using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Subscriptions
{
    public class CancelSubscriptionUseCase : ICancelSubscriptionUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public CancelSubscriptionUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Subscription?> ExecuteAsync(Guid subscriptionId, bool cancelImmediately)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            subscription.AutoRenew = false;
            subscription.CancelledAt = now;

            if (cancelImmediately)
            {
                subscription.Status = SubscriptionStatus.Cancelled;
                subscription.EndedAt = now;
                subscription.CurrentPeriodEnd = now;
            }

            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            return subscription;
        }
    }
}
