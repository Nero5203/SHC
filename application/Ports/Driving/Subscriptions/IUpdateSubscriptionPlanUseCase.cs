using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driving.Subscriptions
{
    public interface IUpdateSubscriptionPlanUseCase
    {
        Task<SubscriptionPlan?> ExecuteAsync(
            Guid subscriptionPlanId,
            string name,
            string? description,
            decimal price,
            string currency,
            BillingInterval billingInterval,
            long storageLimitBytes,
            long? maxFileSizeBytes,
            bool isActive);
    }
}
