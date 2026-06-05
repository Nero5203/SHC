using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IListSubscriptionPlansUseCase
    {
        Task<IReadOnlyList<SubscriptionPlan>> ExecuteAsync(bool activeOnly);
    }
}
