using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IListSubscriptionsUseCase
    {
        Task<IReadOnlyList<Subscription>> ExecuteAsync();
    }
}
