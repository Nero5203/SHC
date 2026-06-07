using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Subscriptions
{
    public class CreateSubscriptionUseCase : ICreateSubscriptionUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public CreateSubscriptionUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Subscription> ExecuteAsync(
            Guid userId,
            Guid subscriptionPlanId,
            DateTime? trialEndsAt,
            bool autoRenew,
            string? providerSubscriptionId)
        {
            var plan = await _subscriptionRepository.GetPlanByIdAsync(subscriptionPlanId);
            if (plan == null || !plan.IsActive)
            {
                throw new InvalidOperationException("Subscription plan is not available.");
            }

            var existingPendingSubscription = await _subscriptionRepository
                .GetPendingSubscriptionByUserIdAndPlanIdAsync(userId, subscriptionPlanId);

            if (existingPendingSubscription != null)
            {
                return existingPendingSubscription;
            }

            var now = DateTime.UtcNow;
            var periodEnd = SubscriptionPeriodCalculator.AddBillingPeriod(now, plan.BillingInterval);

            var subscription = new Subscription
            {
                SubscriptionId = Guid.NewGuid(),
                SubscriptionPlanId = subscriptionPlanId,
                Status = trialEndsAt.HasValue && trialEndsAt.Value > now
                    ? SubscriptionStatus.Trialing
                    : SubscriptionStatus.Pending,
                StartedAt = now,
                CurrentPeriodStart = now,
                CurrentPeriodEnd = periodEnd,
                TrialEndsAt = trialEndsAt,
                AutoRenew = autoRenew,
                ProviderSubscriptionId = providerSubscriptionId
            };

            var userSubscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionId = subscription.SubscriptionId,
                IsOwner = true,
                AssignedAt = now
            };

            await _subscriptionRepository.CreateSubscriptionAsync(subscription, userSubscription);
            return await _subscriptionRepository.GetSubscriptionByIdAsync(subscription.SubscriptionId) ?? subscription;
        }
    }
}
