using Domain.Entities.Purchases;

namespace ports.DrivingPorts.Purchases
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
