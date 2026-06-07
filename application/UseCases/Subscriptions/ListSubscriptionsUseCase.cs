using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class ListSubscriptionsUseCase : IListSubscriptionsUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ListSubscriptionsUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IReadOnlyList<Subscription>> ExecuteAsync()
        {
            return await _subscriptionRepository.GetSubscriptionsAsync();
        }
    }
}
