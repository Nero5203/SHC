using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace ports.DrivingPorts.Purchases
{
    public interface IUpdatePurchaseStatusUseCase
    {
        Task<Purchase?> ExecuteAsync(Guid purchaseId, PurchaseStatus status);
    }
}
