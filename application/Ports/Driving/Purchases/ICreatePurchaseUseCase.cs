using Domain.Entities.Purchases;

namespace application.Ports.Driving.Purchases
{
    public interface ICreatePurchaseUseCase
    {
        Task<Purchase> ExecuteAsync(
            Guid userId,
            Guid subscriptionId,
            decimal amount,
            string currency);
    }
}
