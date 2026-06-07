using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IGetAllPurchasesUseCase
    {
        Task<IReadOnlyList<Purchase>> ExecuteAsync();
    }
}
