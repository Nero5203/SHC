using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface ICancelSubscriptionUseCase
    {
        Task<Subscription?> ExecuteAsync(Guid subscriptionId, bool cancelImmediately);
    }
}
