using application.Dto.Payments;
using application.Ports.Driving.Payments;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/purchases")]
    public class PaymentsController : ControllerBase
    {
        private readonly ICreateCheckoutSessionUseCase _createCheckoutSessionUseCase;

        public PaymentsController(ICreateCheckoutSessionUseCase createCheckoutSessionUseCase)
        {
            _createCheckoutSessionUseCase = createCheckoutSessionUseCase;
        }

        [HttpPost("{purchaseId:guid}/checkout")]
        public async Task<ActionResult<CheckoutSessionResponseDto>> CreateCheckoutSession(
            Guid purchaseId,
            CreateCheckoutSessionDto dto)
        {
            try
            {
                var checkoutResult = await _createCheckoutSessionUseCase.ExecuteAsync(purchaseId, dto.SuccessUrl, dto.CancelUrl);

                return Ok(new CheckoutSessionResponseDto
                {
                    PurchaseId = purchaseId,
                    CheckoutSessionId = checkoutResult.CheckoutSessionId,
                    CheckoutUrl = checkoutResult.CheckoutUrl
                });
            }
            catch (InvalidOperationException ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status503ServiceUnavailable,
                    title: "Stripe is not configured");
            }
        }
    }
}
