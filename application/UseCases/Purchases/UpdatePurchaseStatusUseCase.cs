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

            return purchase;
        }
    }
}
