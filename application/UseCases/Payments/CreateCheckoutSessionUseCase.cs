using Domain.Entities.Purchases;
using application.Ports.Driven.Payments;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Payments;

namespace application.UseCases.Payments
{
    public class CreateCheckoutSessionUseCase : ICreateCheckoutSessionUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public CreateCheckoutSessionUseCase(
            IPurchaseRepository purchaseRepository,
            IPaymentGatewayService paymentGatewayService)
        {
            _purchaseRepository = purchaseRepository;
            _paymentGatewayService = paymentGatewayService;
        }

        public async Task<CheckoutSessionResult> ExecuteAsync(Guid purchaseId, string successUrl, string cancelUrl)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);

            if (purchase == null)
            {
                throw new InvalidOperationException($"Purchase not found: {purchaseId}");
            }

            var checkoutRequest = new PaymentCheckoutRequest
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var checkoutResult = await _paymentGatewayService.CreateCheckoutSessionAsync(checkoutRequest);

            purchase.ProviderCheckoutSessionId = checkoutResult.CheckoutSessionId;
            await _purchaseRepository.UpdateAsync(purchase);

            return checkoutResult;
        }
    }
}
