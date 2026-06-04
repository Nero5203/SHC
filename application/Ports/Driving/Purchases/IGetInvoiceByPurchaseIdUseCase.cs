using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IGetInvoiceByPurchaseIdUseCase
    {
        Task<Invoice?> ExecuteAsync(Guid purchaseId);
    }
}
