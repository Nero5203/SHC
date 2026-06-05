using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IGetActiveUserSubscriptionUseCase
    {
        Task<Subscription?> ExecuteAsync(Guid userId);
    }
}
