using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IGetPurchasesByUserIdUseCase
    {
        Task<IReadOnlyList<Purchase>> ExecuteAsync(Guid userId);
    }
}
