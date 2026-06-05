using application.Dto.Subscriptions;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;

namespace application.UseCases.Subscriptions
{
    public class GetSubscriptionEntitlementsUseCase : IGetSubscriptionEntitlementsUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptionEntitlementsUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionEntitlementResponseDto> ExecuteAsync(Guid userId)
        {
            var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, DateTime.UtcNow);

            if (subscription == null)
            {
                return new SubscriptionEntitlementResponseDto
                {
                    UserId = userId,
                    HasActiveSubscription = false,
                    StorageLimitBytes = 0
                };
            }

            return new SubscriptionEntitlementResponseDto
            {
                UserId = userId,
                SubscriptionId = subscription.SubscriptionId,
                SubscriptionPlanId = subscription.SubscriptionPlanId,
                PlanName = subscription.SubscriptionPlan.Name,
                Status = subscription.Status,
                HasActiveSubscription = true,
                StorageLimitBytes = subscription.SubscriptionPlan.StorageLimitBytes,
                MaxFileSizeBytes = subscription.SubscriptionPlan.MaxFileSizeBytes,
                CurrentPeriodEnd = subscription.CurrentPeriodEnd
            };
        }
    }
}
