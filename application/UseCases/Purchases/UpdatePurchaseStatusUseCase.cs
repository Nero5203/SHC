using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;

namespace application.UseCases.Purchases
{
    public class UpdatePurchaseStatusUseCase : IUpdatePurchaseStatusUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public UpdatePurchaseStatusUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Purchase?> ExecuteAsync(Guid purchaseId, PurchaseStatus status)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);

            if (purchase == null)
            {
                return null;
            }

            await _purchaseRepository.UpdateStatusAsync(purchase, status);

            if (status == PurchaseStatus.Paid)
            {
                var existingInvoice = await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchaseId);
                if (existingInvoice == null)
                {
                    var invoice = InvoiceFactory.CreateFromPurchase(purchase, DateTime.UtcNow);
                    await _purchaseRepository.CreateInvoiceAsync(invoice);
                }
            }

            return purchase;
        }
    }
}
