using Domain.Entities.Purchases;
using ports.DrivenPorts.Purchases;
using ports.DrivingPorts.Purchases;

namespace application.UseCases.Purchases
{
    public class GetPurchasesByUserIdUseCase : IGetPurchasesByUserIdUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public GetPurchasesByUserIdUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<IReadOnlyList<Purchase>> ExecuteAsync(Guid userId)
        {
            return await _purchaseRepository.GetByUserIdAsync(userId);
        }
    }
}
