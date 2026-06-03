using Domain.Entities.Purchases;

namespace ports.DrivingPorts.Purchases
{
    public interface IGetInvoiceByPurchaseIdUseCase
    {
        Task<Invoice?> ExecuteAsync(Guid purchaseId);
    }
}
