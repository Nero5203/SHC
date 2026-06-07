using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;
using Domain.Entities.Purchases;

namespace application.UseCases.Purchases
{
    public class GetAllPurchasesUseCase : IGetAllPurchasesUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public GetAllPurchasesUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<IReadOnlyList<Purchase>> ExecuteAsync()
        {
            return await _purchaseRepository.GetAllAsync();
        }
    }
}
