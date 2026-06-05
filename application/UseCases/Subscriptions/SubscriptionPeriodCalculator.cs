using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Subscriptions
{
    internal static class SubscriptionPeriodCalculator
    {
        public static DateTime AddBillingPeriod(DateTime start, BillingInterval billingInterval)
        {
            return billingInterval switch
            {
                BillingInterval.Monthly => start.AddMonths(1),
                BillingInterval.Yearly => start.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(billingInterval), billingInterval, null)
            };
        }
    }
}
