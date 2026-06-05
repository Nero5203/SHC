using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class GetSubscriptionByIdUseCase : IGetSubscriptionByIdUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetSubscriptionByIdUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Subscription?> ExecuteAsync(Guid subscriptionId)
        {
            return await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
        }
    }
}
