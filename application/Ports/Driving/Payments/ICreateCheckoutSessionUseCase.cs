using application.Ports.Driven.Payments;

namespace application.Ports.Driving.Payments
{
    public interface ICreateCheckoutSessionUseCase
    {
        Task<CheckoutSessionResult> ExecuteAsync(Guid purchaseId, string successUrl, string cancelUrl);
    }
}
