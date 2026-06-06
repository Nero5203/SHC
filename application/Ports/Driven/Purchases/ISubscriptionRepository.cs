using Domain.Entities.Purchases;

namespace application.Ports.Driven.Purchases
{
    public interface ISubscriptionRepository
    {
        Task CreatePlanAsync(SubscriptionPlan plan);
        Task<SubscriptionPlan?> GetPlanByIdAsync(Guid subscriptionPlanId);
        Task<IReadOnlyList<SubscriptionPlan>> GetPlansAsync(bool activeOnly);
        Task UpdatePlanAsync(SubscriptionPlan plan);

        Task CreateSubscriptionAsync(Subscription subscription, UserSubscription userSubscription);
        Task<IReadOnlyList<Subscription>> GetSubscriptionsAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(Guid subscriptionId);
        Task<IReadOnlyList<Subscription>> GetSubscriptionsByUserIdAsync(Guid userId);
        Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, DateTime utcNow);
        Task UpdateSubscriptionAsync(Subscription subscription);
    }
}
