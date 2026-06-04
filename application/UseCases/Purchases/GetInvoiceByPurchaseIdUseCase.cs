using Domain.Entities.Purchases;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;

namespace application.UseCases.Purchases
{
    public class GetInvoiceByPurchaseIdUseCase : IGetInvoiceByPurchaseIdUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public GetInvoiceByPurchaseIdUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Invoice?> ExecuteAsync(Guid purchaseId)
        {
            return await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchaseId);
        }
    }
}
