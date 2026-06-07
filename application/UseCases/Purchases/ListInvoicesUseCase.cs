using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;
using Domain.Entities.Purchases;

namespace application.UseCases.Purchases
{
    public class ListInvoicesUseCase : IListInvoicesUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;

        public ListInvoicesUseCase(IPurchaseRepository purchaseRepository)
        {
            _purchaseRepository = purchaseRepository;
        }

        public async Task<IReadOnlyList<Invoice>> ExecuteAsync()
        {
            return await _purchaseRepository.GetAllInvoicesAsync();
        }
    }
}
