using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IGetSubscriptionPlanByIdUseCase
    {
        Task<SubscriptionPlan?> ExecuteAsync(Guid subscriptionPlanId);
    }
}
