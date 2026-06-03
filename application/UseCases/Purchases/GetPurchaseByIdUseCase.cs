using Domain.Entities.Purchases;
using ports.DrivenPorts.Purchases;
using ports.DrivingPorts.Purchases;

namespace application.UseCases.Purchases
{
    public class GetPurchaseByIdUseCase : IGetPurchaseByIdUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public GetPurchaseByIdUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<Purchase?> ExecuteAsync(Guid purchaseId)
        {
            return await _purchaseRepository.GetByIdAsync(purchaseId);
        }
    }
}
