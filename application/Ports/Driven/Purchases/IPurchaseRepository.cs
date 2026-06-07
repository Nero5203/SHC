using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driven.Purchases
{
    public interface IPurchaseRepository
    {
        Task CreateAsync(Purchase purchase);
        Task<IReadOnlyList<Purchase>> GetAllAsync();
        Task<Purchase?> GetByIdAsync(Guid purchaseId);
        Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId);
        Task<Purchase?> GetPendingBySubscriptionIdAsync(Guid subscriptionId);
        Task<Invoice?> GetInvoiceByPurchaseIdAsync(Guid purchaseId);
        Task UpdateStatusAsync(Purchase purchase, PurchaseStatus status);
        Task UpdateAsync(Purchase purchase);
    }
}
