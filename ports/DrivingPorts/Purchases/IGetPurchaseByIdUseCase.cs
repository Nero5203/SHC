using Domain.Entities.Purchases;

namespace ports.DrivingPorts.Purchases
{
    public interface IGetPurchaseByIdUseCase
    {
        Task<Purchase?> ExecuteAsync(Guid purchaseId);
    }
}
