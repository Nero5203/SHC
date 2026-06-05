using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class GetSubscriptionPlanByIdUseCase : IGetSubscriptionPlanByIdUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptionPlanByIdUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionPlan?> ExecuteAsync(Guid subscriptionPlanId)
        {
            return await _subscriptionRepository.GetPlanByIdAsync(subscriptionPlanId);
        }
    }
}
