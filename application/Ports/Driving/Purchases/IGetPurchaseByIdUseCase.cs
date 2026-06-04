using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface IGetPurchaseByIdUseCase
    {
        Task<Purchase?> ExecuteAsync(Guid purchaseId);
    }
}
