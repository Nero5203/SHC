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
        Task<IReadOnlyList<Invoice>> GetAllInvoicesAsync();
        Task<Invoice?> GetInvoiceByPurchaseIdAsync(Guid purchaseId);
        Task CreateInvoiceAsync(Invoice invoice);
        Task UpdateStatusAsync(Purchase purchase, PurchaseStatus status);
        Task UpdateAsync(Purchase purchase);
    }
}
