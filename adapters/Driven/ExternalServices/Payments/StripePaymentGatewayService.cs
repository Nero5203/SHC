using Stripe;
using Stripe.Checkout;
using application.Ports.Driven.Payments;
using Domain.Entities.Purchases.Enums;
using Microsoft.Extensions.Configuration;

namespace adapters.Driven.ExternalServices.Payments
{
    public class StripePaymentGatewayService : IPaymentGatewayService
    {
        private readonly SessionService _sessionService;
        private readonly string _webhookSecret;

        public StripePaymentGatewayService(IConfiguration configuration)
        {
            var secretKey = configuration["Stripe:SecretKey"];
            _webhookSecret = configuration["Stripe:WebhookSecret"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                throw new InvalidOperationException("Stripe secret key is not configured. Set Stripe:SecretKey in configuration.");
            }

            var client = new StripeClient(secretKey);
            _sessionService = new SessionService(client);
        }

        public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request)
        {
            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "purchaseId", request.PurchaseId.ToString() }
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = request.Currency.ToLowerInvariant(),
                            UnitAmount = (long)(request.Amount * 100m),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Subscription Purchase"
                            }
                        }
                    }
                }
            };

            var session = await _sessionService.CreateAsync(sessionOptions);

            return new CheckoutSessionResult
            {
                CheckoutSessionId = session.Id ?? string.Empty,
                CheckoutUrl = session.Url ?? string.Empty
            };
        }

        public Task<PaymentGatewayWebhookResult> ProcessWebhookAsync(string payload, string signatureHeader)
        {
            var result = new PaymentGatewayWebhookResult();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _webhookSecret);
                result.Handled = true;

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session?.Metadata == null || !session.Metadata.TryGetValue("purchaseId", out var purchaseIdValue))
                    {
                        return Task.FromResult(result);
                    }

                    if (!Guid.TryParse(purchaseIdValue, out var purchaseId))
                    {
                        return Task.FromResult(result);
                    }

                    var status = session.PaymentStatus == "paid" ? PurchaseStatus.Paid : PurchaseStatus.Pending;

                    result.PurchaseId = purchaseId;
                    result.PurchaseStatus = status;
                    result.ProviderPaymentIntentId = session.PaymentIntentId;
                    return Task.FromResult(result);
                }

                if (stripeEvent.Type == "checkout.session.expired")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session?.Metadata == null || !session.Metadata.TryGetValue("purchaseId", out var purchaseIdValue))
                    {
                        return Task.FromResult(result);
                    }

                    if (!Guid.TryParse(purchaseIdValue, out var purchaseId))
                    {
                        return Task.FromResult(result);
                    }

                    result.PurchaseId = purchaseId;
                    result.PurchaseStatus = PurchaseStatus.Cancelled;
                    return Task.FromResult(result);
                }

                if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent == null)
                    {
                        return Task.FromResult(result);
                    }

                    if (intent.Metadata == null || !intent.Metadata.TryGetValue("purchaseId", out var purchaseIdValue))
                    {
                        return Task.FromResult(result);
                    }

                    if (!Guid.TryParse(purchaseIdValue, out var purchaseId))
                    {
                        return Task.FromResult(result);
                    }

                    result.PurchaseId = purchaseId;
                    result.PurchaseStatus = PurchaseStatus.Failed;
                    result.ProviderPaymentIntentId = intent.Id;
                    return Task.FromResult(result);
                }

                return Task.FromResult(result);
            }
            catch (StripeException)
            {
                return Task.FromResult(new PaymentGatewayWebhookResult { Handled = false });
            }
        }
    }
}
