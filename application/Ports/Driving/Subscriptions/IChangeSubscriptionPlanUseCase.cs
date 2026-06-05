using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IChangeSubscriptionPlanUseCase
    {
        Task<Subscription?> ExecuteAsync(Guid subscriptionId, Guid subscriptionPlanId);
    }
}
