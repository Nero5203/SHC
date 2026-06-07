using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IGenerateInvoiceForPurchaseUseCase
    {
        Task<Invoice?> ExecuteAsync(Guid purchaseId);
    }
}
