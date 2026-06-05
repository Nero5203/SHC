using Domain.Entities.Purchases;

namespace application.Ports.Driving.Subscriptions
{
    public interface IGetUserSubscriptionsUseCase
    {
        Task<IReadOnlyList<Subscription>> ExecuteAsync(Guid userId);
    }
}
