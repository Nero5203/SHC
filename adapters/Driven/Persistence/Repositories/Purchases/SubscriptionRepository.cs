using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Purchases;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Purchases
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ShcDbContext _context;

        public SubscriptionRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreatePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();
        }

        public async Task<SubscriptionPlan?> GetPlanByIdAsync(Guid subscriptionPlanId)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(plan => plan.SubscriptionPlanId == subscriptionPlanId);
        }

        public async Task<IReadOnlyList<SubscriptionPlan>> GetPlansAsync(bool activeOnly)
        {
            var query = _context.SubscriptionPlans.AsQueryable();

            if (activeOnly)
            {
                query = query.Where(plan => plan.IsActive);
            }

            return await query
                .OrderBy(plan => plan.Price)
                .ThenBy(plan => plan.Name)
                .ToListAsync();
        }

        public async Task UpdatePlanAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSubscriptionAsync(Subscription subscription, UserSubscription userSubscription)
        {
            _context.Subscriptions.Add(subscription);
            _context.UserSubscriptions.Add(userSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Subscription>> GetSubscriptionsAsync()
        {
            return await _context.Subscriptions
                .Include(subscription => subscription.SubscriptionPlan)
                .Include(subscription => subscription.UserSubscriptions)
                .OrderByDescending(subscription => subscription.StartedAt)
                .ToListAsync();
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            return await _context.Subscriptions
                .Include(subscription => subscription.SubscriptionPlan)
                .Include(subscription => subscription.UserSubscriptions)
                .FirstOrDefaultAsync(subscription => subscription.SubscriptionId == subscriptionId);
        }

        public async Task<IReadOnlyList<Subscription>> GetSubscriptionsByUserIdAsync(Guid userId)
        {
            return await _context.UserSubscriptions
                .Where(userSubscription => userSubscription.UserId == userId && userSubscription.RemovedAt == null)
                .Select(userSubscription => userSubscription.Subscription)
                .Include(subscription => subscription.SubscriptionPlan)
                .Include(subscription => subscription.UserSubscriptions)
                .OrderByDescending(subscription => subscription.StartedAt)
                .ToListAsync();
        }

        public async Task<Subscription?> GetPendingSubscriptionByUserIdAndPlanIdAsync(Guid userId, Guid subscriptionPlanId)
        {
            return await _context.UserSubscriptions
                .Where(userSubscription => userSubscription.UserId == userId && userSubscription.RemovedAt == null)
                .Select(userSubscription => userSubscription.Subscription)
                .Include(subscription => subscription.SubscriptionPlan)
                .Include(subscription => subscription.UserSubscriptions)
                .Where(subscription =>
                    subscription.SubscriptionPlanId == subscriptionPlanId &&
                    subscription.Status == SubscriptionStatus.Pending &&
                    subscription.EndedAt == null)
                .OrderByDescending(subscription => subscription.StartedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, DateTime utcNow)
        {
            return await _context.UserSubscriptions
                .Where(userSubscription => userSubscription.UserId == userId && userSubscription.RemovedAt == null)
                .Select(userSubscription => userSubscription.Subscription)
                .Include(subscription => subscription.SubscriptionPlan)
                .Include(subscription => subscription.UserSubscriptions)
                .Where(subscription =>
                    (subscription.Status == SubscriptionStatus.Active || subscription.Status == SubscriptionStatus.Trialing) &&
                    subscription.CurrentPeriodStart <= utcNow &&
                    subscription.CurrentPeriodEnd > utcNow &&
                    subscription.EndedAt == null)
                .OrderByDescending(subscription => subscription.CurrentPeriodEnd)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateSubscriptionAsync(Subscription subscription)
        {
            _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
