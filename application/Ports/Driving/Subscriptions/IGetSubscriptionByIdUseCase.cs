using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IGetSubscriptionByIdUseCase
    {
        Task<Subscription?> ExecuteAsync(Guid subscriptionId);
    }
}
