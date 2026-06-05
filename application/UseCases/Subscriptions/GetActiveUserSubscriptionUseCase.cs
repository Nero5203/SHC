using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;

namespace application.UseCases.Subscriptions
{
    public class GetActiveUserSubscriptionUseCase : IGetActiveUserSubscriptionUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public GetActiveUserSubscriptionUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Subscription?> ExecuteAsync(Guid userId)
        {
            return await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, DateTime.UtcNow);
        }
    }
}
