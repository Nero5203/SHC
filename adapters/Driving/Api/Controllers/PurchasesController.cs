using application.Dto.Purchases;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using application.Ports.Driving.Purchases;

namespace api.Controllers
{
    [ApiController]
    [Route("api/purchases")]
    public class PurchasesController : ControllerBase
    {
        private readonly ICreatePurchaseUseCase _createPurchaseUseCase;
        private readonly IGetAllPurchasesUseCase _getAllPurchasesUseCase;
        private readonly IGetPurchaseByIdUseCase _getPurchaseByIdUseCase;
        private readonly IGetPurchasesByUserIdUseCase _getPurchasesByUserIdUseCase;
        private readonly IUpdatePurchaseStatusUseCase _updatePurchaseStatusUseCase;
        private readonly IGetInvoiceByPurchaseIdUseCase _getInvoiceByPurchaseIdUseCase;
        private readonly IMapper _mapper;

        public PurchasesController(
            ICreatePurchaseUseCase createPurchaseUseCase,
            IGetAllPurchasesUseCase getAllPurchasesUseCase,
            IGetPurchaseByIdUseCase getPurchaseByIdUseCase,
            IGetPurchasesByUserIdUseCase getPurchasesByUserIdUseCase,
            IUpdatePurchaseStatusUseCase updatePurchaseStatusUseCase,
            IGetInvoiceByPurchaseIdUseCase getInvoiceByPurchaseIdUseCase,
            IMapper mapper)
        {
            _createPurchaseUseCase = createPurchaseUseCase;
            _getAllPurchasesUseCase = getAllPurchasesUseCase;
            _getPurchaseByIdUseCase = getPurchaseByIdUseCase;
            _getPurchasesByUserIdUseCase = getPurchasesByUserIdUseCase;
            _updatePurchaseStatusUseCase = updatePurchaseStatusUseCase;
            _getInvoiceByPurchaseIdUseCase = getInvoiceByPurchaseIdUseCase;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseResponseDto>> CreatePurchase(CreatePurchaseDto dto)
        {
            try
            {
                var purchase = await _createPurchaseUseCase.ExecuteAsync(
                    dto.UserId,
                    dto.SubscriptionId);

                var response = MapPurchase(purchase);

                return CreatedAtAction(nameof(GetPurchaseById), new { purchaseId = purchase.PurchaseId }, response);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<PurchaseResponseDto>>> GetAllPurchases()
        {
            var purchases = await _getAllPurchasesUseCase.ExecuteAsync();

            var response = purchases
                .Select(MapPurchase)
                .ToList();

            return Ok(response);
        }

        [HttpGet("{purchaseId:guid}")]
        public async Task<ActionResult<PurchaseResponseDto>> GetPurchaseById(Guid purchaseId)
        {
            var purchase = await _getPurchaseByIdUseCase.ExecuteAsync(purchaseId);

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(MapPurchase(purchase));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<PurchaseResponseDto>>> GetPurchasesByUserId(Guid userId)
        {
            var purchases = await _getPurchasesByUserIdUseCase.ExecuteAsync(userId);

            var response = purchases
                .Select(MapPurchase)
                .ToList();

            return Ok(response);
        }

        [HttpPut("{purchaseId:guid}/status")]
        public async Task<ActionResult<PurchaseResponseDto>> UpdatePurchaseStatus(
            Guid purchaseId,
            UpdatePurchaseStatusDto dto)
        {
            var purchase = await _updatePurchaseStatusUseCase.ExecuteAsync(purchaseId, dto.Status);

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(MapPurchase(purchase));
        }

        [HttpGet("{purchaseId:guid}/invoice")]
        public async Task<ActionResult<InvoiceResponseDto>> GetInvoiceByPurchaseId(Guid purchaseId)
        {
            var invoice = await _getInvoiceByPurchaseIdUseCase.ExecuteAsync(purchaseId);

            if (invoice == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<InvoiceResponseDto>(invoice);

            return Ok(response);
        }

        private static PurchaseResponseDto MapPurchase(Domain.Entities.Purchases.Purchase purchase)
        {
            return new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                PurchasedAt = purchase.PurchasedAt,
                Status = purchase.Status
            };
        }
    }
}
