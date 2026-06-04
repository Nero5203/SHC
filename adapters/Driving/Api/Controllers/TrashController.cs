using application.Dto.Trash;
using application.Ports.Driving.Trash;
using Domain.Entities.Trash;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/trash")]
    public class TrashController : ControllerBase
    {
        private readonly ICreateTrashedItemUseCase _createTrashedItemUseCase;
        private readonly IGetTrashedItemByIdUseCase _getTrashedItemByIdUseCase;
        private readonly IGetTrashedItemsByUserIdUseCase _getTrashedItemsByUserIdUseCase;
        private readonly IRestoreTrashedItemUseCase _restoreTrashedItemUseCase;
        private readonly IPermanentlyDeleteTrashedItemUseCase _permanentlyDeleteTrashedItemUseCase;

        public TrashController(
            ICreateTrashedItemUseCase createTrashedItemUseCase,
            IGetTrashedItemByIdUseCase getTrashedItemByIdUseCase,
            IGetTrashedItemsByUserIdUseCase getTrashedItemsByUserIdUseCase,
            IRestoreTrashedItemUseCase restoreTrashedItemUseCase,
            IPermanentlyDeleteTrashedItemUseCase permanentlyDeleteTrashedItemUseCase)
        {
            _createTrashedItemUseCase = createTrashedItemUseCase;
            _getTrashedItemByIdUseCase = getTrashedItemByIdUseCase;
            _getTrashedItemsByUserIdUseCase = getTrashedItemsByUserIdUseCase;
            _restoreTrashedItemUseCase = restoreTrashedItemUseCase;
            _permanentlyDeleteTrashedItemUseCase = permanentlyDeleteTrashedItemUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<TrashedItemResponseDto>> CreateTrashedItem(CreateTrashedItemDto dto)
        {
            var trashedItem = await _createTrashedItemUseCase.ExecuteAsync(
                dto.UserId,
                dto.OriginalItemId,
                dto.ItemType,
                dto.Name,
                dto.OriginalPath,
                dto.OriginalParentId,
                dto.Size,
                dto.ExpiresAt);

            return CreatedAtAction(
                nameof(GetTrashedItemById),
                new { trashedItemId = trashedItem.TrashedItemId },
                MapTrashedItem(trashedItem));
        }

        [HttpGet("{trashedItemId:guid}")]
        public async Task<ActionResult<TrashedItemResponseDto>> GetTrashedItemById(Guid trashedItemId)
        {
            var trashedItem = await _getTrashedItemByIdUseCase.ExecuteAsync(trashedItemId);

            if (trashedItem == null)
            {
                return NotFound();
            }

            return Ok(MapTrashedItem(trashedItem));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<TrashedItemResponseDto>>> GetTrashedItemsByUserId(Guid userId)
        {
            var trashedItems = await _getTrashedItemsByUserIdUseCase.ExecuteAsync(userId);

            var response = trashedItems
                .Select(MapTrashedItem)
                .ToList();

            return Ok(response);
        }

        [HttpPut("{trashedItemId:guid}/restore")]
        public async Task<ActionResult<TrashedItemResponseDto>> RestoreTrashedItem(Guid trashedItemId)
        {
            var trashedItem = await _restoreTrashedItemUseCase.ExecuteAsync(trashedItemId);

            if (trashedItem == null)
            {
                return NotFound();
            }

            return Ok(MapTrashedItem(trashedItem));
        }

        [HttpDelete("{trashedItemId:guid}")]
        public async Task<IActionResult> PermanentlyDeleteTrashedItem(Guid trashedItemId)
        {
            var deleted = await _permanentlyDeleteTrashedItemUseCase.ExecuteAsync(trashedItemId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static TrashedItemResponseDto MapTrashedItem(TrashedItem trashedItem)
        {
            return new TrashedItemResponseDto
            {
                TrashedItemId = trashedItem.TrashedItemId,
                UserId = trashedItem.UserId,
                OriginalItemId = trashedItem.OriginalItemId,
                ItemType = trashedItem.ItemType,
                Name = trashedItem.Name,
                OriginalPath = trashedItem.OriginalPath,
                OriginalParentId = trashedItem.OriginalParentId,
                Size = trashedItem.Size,
                DeletedAt = trashedItem.DeletedAt,
                RestoredAt = trashedItem.RestoredAt,
                ExpiresAt = trashedItem.ExpiresAt
            };
        }
    }
}
