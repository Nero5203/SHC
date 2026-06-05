using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface ICreateSubscriptionUseCase
    {
        Task<Subscription> ExecuteAsync(
            Guid userId,
            Guid subscriptionPlanId,
            DateTime? trialEndsAt,
            bool autoRenew,
            string? providerSubscriptionId);
    }
}
