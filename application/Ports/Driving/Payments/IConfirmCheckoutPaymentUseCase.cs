using Domain.Entities.Purchases;

namespace application.Ports.Driving.Payments
{
    public interface IConfirmCheckoutPaymentUseCase
    {
        Task<Purchase?> ExecuteAsync(Guid purchaseId);
    }
}
