using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;
using Domain.Entities.Purchases;

namespace application.UseCases.Purchases
{
    public class GenerateInvoiceForPurchaseUseCase : IGenerateInvoiceForPurchaseUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public GenerateInvoiceForPurchaseUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Invoice?> ExecuteAsync(Guid purchaseId)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);
            if (purchase == null)
            {
                return null;
            }

            var existingInvoice = await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchaseId);
            if (existingInvoice != null)
            {
                return existingInvoice;
            }

            var invoice = InvoiceFactory.CreateFromPurchase(purchase, DateTime.UtcNow);
            await _purchaseRepository.CreateInvoiceAsync(invoice);

            return invoice;
        }
    }
}
