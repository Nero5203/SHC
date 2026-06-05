using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driving.Subscriptions
{
    public interface ICreateSubscriptionPlanUseCase
    {
        Task<SubscriptionPlan> ExecuteAsync(
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
