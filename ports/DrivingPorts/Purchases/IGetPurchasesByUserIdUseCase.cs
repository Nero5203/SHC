using Domain.Entities.Purchases;

namespace ports.DrivingPorts.Purchases
{
    public interface IGetPurchasesByUserIdUseCase
    {
        Task<IReadOnlyList<Purchase>> ExecuteAsync(Guid userId);
    }
}
