using application.Ports.Driving.Payments;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/webhooks")]

    public class WebhookController : ControllerBase
    {
        private readonly IProcessPaymentWebhookUseCase _processPaymentWebhookUseCase;

        public WebhookController(IProcessPaymentWebhookUseCase processPaymentWebhookUseCase)
        {
            _processPaymentWebhookUseCase = processPaymentWebhookUseCase;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signatureHeader = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(signatureHeader))
            {
                return BadRequest("Missing Stripe signature header.");
            }

            var handled = await _processPaymentWebhookUseCase.ExecuteAsync(payload, signatureHeader);
            if (!handled)
            {
                return BadRequest("Webhook event could not be processed.");
            }

            return Ok();
        }
    }
}
