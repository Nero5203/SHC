using application.Ports.Driven.Purchases;
using application.Ports.Driving.Subscriptions;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Subscriptions
{
    public class CreateSubscriptionPlanUseCase : ICreateSubscriptionPlanUseCase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public CreateSubscriptionPlanUseCase(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<SubscriptionPlan> ExecuteAsync(
            string name,
            string? description,
            decimal price,
            string currency,
            BillingInterval billingInterval,
            long storageLimitBytes,
            long? maxFileSizeBytes,
            bool isActive)
        {
            ValidatePlan(name, price, currency, storageLimitBytes, maxFileSizeBytes);

            var now = DateTime.UtcNow;
            var plan = new SubscriptionPlan
            {
                SubscriptionPlanId = Guid.NewGuid(),
                Name = name.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                Price = price,
                Currency = currency.Trim().ToUpperInvariant(),
                BillingInterval = billingInterval,
                StorageLimitBytes = storageLimitBytes,
                MaxFileSizeBytes = maxFileSizeBytes,
                IsActive = isActive,
                CreatedAt = now,
                UpdatedAt = now
            };

            await _subscriptionRepository.CreatePlanAsync(plan);
            return plan;
        }

        internal static void ValidatePlan(
            string name,
            decimal price,
            string currency,
            long storageLimitBytes,
            long? maxFileSizeBytes)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Subscription plan name is required.", nameof(name));
            }

            if (price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(price), "Subscription plan price cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("Currency is required.", nameof(currency));
            }

            if (storageLimitBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(storageLimitBytes), "Storage limit must be greater than zero.");
            }

            if (maxFileSizeBytes <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFileSizeBytes), "Max file size must be greater than zero when provided.");
            }
        }
    }
}
