using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driving.Purchases
{
    public interface IUpdatePurchaseStatusUseCase
    {
        Task<Purchase?> ExecuteAsync(Guid purchaseId, PurchaseStatus status);
    }
}
