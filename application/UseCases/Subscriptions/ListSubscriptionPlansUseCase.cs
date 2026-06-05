using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class ListSubscriptionPlansUseCase : IListSubscriptionPlansUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ListSubscriptionPlansUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<IReadOnlyList<SubscriptionPlan>> ExecuteAsync(bool activeOnly)
        {
            return await _subscriptionRepository.GetPlansAsync(activeOnly);
        }
    }
}
