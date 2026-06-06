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
        private readonly IGetPurchaseByIdUseCase _getPurchaseByIdUseCase;
        private readonly IGetPurchasesByUserIdUseCase _getPurchasesByUserIdUseCase;
        private readonly IUpdatePurchaseStatusUseCase _updatePurchaseStatusUseCase;
        private readonly IGetInvoiceByPurchaseIdUseCase _getInvoiceByPurchaseIdUseCase;
        private readonly IMapper _mapper;

        public PurchasesController(
            ICreatePurchaseUseCase createPurchaseUseCase,
            IGetPurchaseByIdUseCase getPurchaseByIdUseCase,
            IGetPurchasesByUserIdUseCase getPurchasesByUserIdUseCase,
            IUpdatePurchaseStatusUseCase updatePurchaseStatusUseCase,
            IGetInvoiceByPurchaseIdUseCase getInvoiceByPurchaseIdUseCase,
            IMapper mapper)
        {
            _createPurchaseUseCase = createPurchaseUseCase;
            _getPurchaseByIdUseCase = getPurchaseByIdUseCase;
            _getPurchasesByUserIdUseCase = getPurchasesByUserIdUseCase;
            _updatePurchaseStatusUseCase = updatePurchaseStatusUseCase;
            _getInvoiceByPurchaseIdUseCase = getInvoiceByPurchaseIdUseCase;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseResponseDto>> CreatePurchase(CreatePurchaseDto dto)
        {
            var purchase = await _createPurchaseUseCase.ExecuteAsync(
                dto.UserId,
                dto.SubscriptionId,
                dto.Amount,
                dto.Currency);

            var response = new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                PurchasedAt = purchase.PurchasedAt,
                Status = purchase.Status
            };

            return CreatedAtAction(nameof(GetPurchaseById), new { purchaseId = purchase.PurchaseId }, response);
        }

        [HttpGet("{purchaseId:guid}")]
        public async Task<ActionResult<PurchaseResponseDto>> GetPurchaseById(Guid purchaseId)
        {
            var purchase = await _getPurchaseByIdUseCase.ExecuteAsync(purchaseId);

            if (purchase == null)
            {
                return NotFound();
            }

            var response = new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                PurchasedAt = purchase.PurchasedAt,
                Status = purchase.Status
            };

            return Ok(response);
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<PurchaseResponseDto>>> GetPurchasesByUserId(Guid userId)
        {
            var purchases = await _getPurchasesByUserIdUseCase.ExecuteAsync(userId);

            var response = purchases.Select(purchase => new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                PurchasedAt = purchase.PurchasedAt,
                Status = purchase.Status
            }).ToList();

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

            var response = new PurchaseResponseDto
            {
                PurchaseId = purchase.PurchaseId,
                UserId = purchase.UserId,
                SubscriptionId = purchase.SubscriptionId,
                Amount = purchase.Amount,
                Currency = purchase.Currency,
                PurchasedAt = purchase.PurchasedAt,
                Status = purchase.Status
            };

            return Ok(response);
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
    }
}
