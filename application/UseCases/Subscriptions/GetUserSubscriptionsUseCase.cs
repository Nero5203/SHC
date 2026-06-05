using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class GetUserSubscriptionsUseCase : IGetUserSubscriptionsUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetUserSubscriptionsUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IReadOnlyList<Subscription>> ExecuteAsync(Guid userId)
        {
            return await _subscriptionRepository.GetSubscriptionsByUserIdAsync(userId);
        }
    }
}
