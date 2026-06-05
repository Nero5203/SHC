using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Subscriptions
{
    public class UpdateSubscriptionPlanUseCase : IUpdateSubscriptionPlanUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public UpdateSubscriptionPlanUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionPlan?> ExecuteAsync(
            Guid subscriptionPlanId,
            string name,
            string? description,
            decimal price,
            string currency,
            BillingInterval billingInterval,
            long storageLimitBytes,
            long? maxFileSizeBytes,
            bool isActive)
        {
            CreateSubscriptionPlanUseCase.ValidatePlan(name, price, currency, storageLimitBytes, maxFileSizeBytes);

            var plan = await _subscriptionRepository.GetPlanByIdAsync(subscriptionPlanId);
            if (plan == null)
            {
                return null;
            }

            plan.Name = name.Trim();
            plan.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            plan.Price = price;
            plan.Currency = currency.Trim().ToUpperInvariant();
            plan.BillingInterval = billingInterval;
            plan.StorageLimitBytes = storageLimitBytes;
            plan.MaxFileSizeBytes = maxFileSizeBytes;
            plan.IsActive = isActive;
            plan.UpdatedAt = DateTime.UtcNow;

            await _subscriptionRepository.UpdatePlanAsync(plan);
            return plan;
        }
    }
}
