using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class ChangeSubscriptionPlanUseCase : IChangeSubscriptionPlanUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ChangeSubscriptionPlanUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Subscription?> ExecuteAsync(Guid subscriptionId, Guid subscriptionPlanId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
            {
                return null;
            }

            var plan = await _subscriptionRepository.GetPlanByIdAsync(subscriptionPlanId);
            if (plan == null || !plan.IsActive)
            {
                throw new InvalidOperationException("Subscription plan is not available.");
            }

            subscription.SubscriptionPlanId = subscriptionPlanId;
            subscription.SubscriptionPlan = plan;
            subscription.CurrentPeriodEnd = SubscriptionPeriodCalculator.AddBillingPeriod(
                subscription.CurrentPeriodStart,
                plan.BillingInterval);

            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
            return subscription;
        }
    }
}
