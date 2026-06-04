using application.Dto.LinkSharing;
using Domain.Entities.LinkSharing;
using Microsoft.AspNetCore.Mvc;
using application.Ports.Driving.LinkSharing;

namespace api.Controllers.LinkSharing
{
    [ApiController]
    [Route("api/shared-links")]
    public class SharedLinksController : ControllerBase
    {
        private readonly ICreateSharedLinkUseCase _createSharedLinkUseCase;
        private readonly IGetSharedLinkByIdUseCase _getSharedLinkByIdUseCase;
        private readonly IGetSharedLinkByTokenUseCase _getSharedLinkByTokenUseCase;
        private readonly IGetSharedLinksByUserIdUseCase _getSharedLinksByUserIdUseCase;
        private readonly IUpdateSharedLinkUseCase _updateSharedLinkUseCase;
        private readonly IDeactivateSharedLinkUseCase _deactivateSharedLinkUseCase;

        public SharedLinksController(
            ICreateSharedLinkUseCase createSharedLinkUseCase,
            IGetSharedLinkByIdUseCase getSharedLinkByIdUseCase,
            IGetSharedLinkByTokenUseCase getSharedLinkByTokenUseCase,
            IGetSharedLinksByUserIdUseCase getSharedLinksByUserIdUseCase,
            IUpdateSharedLinkUseCase updateSharedLinkUseCase,
            IDeactivateSharedLinkUseCase deactivateSharedLinkUseCase)
        {
            _createSharedLinkUseCase = createSharedLinkUseCase;
            _getSharedLinkByIdUseCase = getSharedLinkByIdUseCase;
            _getSharedLinkByTokenUseCase = getSharedLinkByTokenUseCase;
            _getSharedLinksByUserIdUseCase = getSharedLinksByUserIdUseCase;
            _updateSharedLinkUseCase = updateSharedLinkUseCase;
            _deactivateSharedLinkUseCase = deactivateSharedLinkUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<SharedLinkResponseDto>> CreateSharedLink(CreateSharedLinkDto dto)
        {
            var sharedLink = await _createSharedLinkUseCase.ExecuteAsync(
                dto.UserId,
                dto.TargetId,
                dto.TargetType,
                dto.ExpirationDate,
                dto.CanView,
                dto.CanEdit,
                dto.AllowDownload);

            var response = MapToResponse(sharedLink);

            return CreatedAtAction(nameof(GetSharedLinkById), new { sharedLinkId = sharedLink.SharedLinkId }, response);
        }

        [HttpGet("{sharedLinkId:guid}")]
        public async Task<ActionResult<SharedLinkResponseDto>> GetSharedLinkById(Guid sharedLinkId)
        {
            var sharedLink = await _getSharedLinkByIdUseCase.ExecuteAsync(sharedLinkId);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(sharedLink));
        }

        [HttpGet("token/{tokenUrl}")]
        public async Task<ActionResult<SharedLinkResponseDto>> GetSharedLinkByToken(string tokenUrl)
        {
            var sharedLink = await _getSharedLinkByTokenUseCase.ExecuteAsync(tokenUrl);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(sharedLink));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<SharedLinkResponseDto>>> GetSharedLinksByUserId(Guid userId)
        {
            var sharedLinks = await _getSharedLinksByUserIdUseCase.ExecuteAsync(userId);

            var response = sharedLinks
                .Select(MapToResponse)
                .ToList();

            return Ok(response);
        }

        [HttpPut("{sharedLinkId:guid}")]
        public async Task<ActionResult<SharedLinkResponseDto>> UpdateSharedLink(
            Guid sharedLinkId,
            UpdateSharedLinkDto dto)
        {
            var sharedLink = await _updateSharedLinkUseCase.ExecuteAsync(
                sharedLinkId,
                dto.ExpirationDate,
                dto.IsActive,
                dto.CanView,
                dto.CanEdit,
                dto.AllowDownload);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(sharedLink));
        }

        [HttpPut("{sharedLinkId:guid}/deactivate")]
        public async Task<ActionResult<SharedLinkResponseDto>> DeactivateSharedLink(Guid sharedLinkId)
        {
            var sharedLink = await _deactivateSharedLinkUseCase.ExecuteAsync(sharedLinkId);

            if (sharedLink == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(sharedLink));
        }

        private static SharedLinkResponseDto MapToResponse(SharedLink sharedLink)
        {
            return new SharedLinkResponseDto
            {
                SharedLinkId = sharedLink.SharedLinkId,
                TokenUrl = sharedLink.TokenUrl,
                UserId = sharedLink.UserId,
                TargetId = sharedLink.TargetId,
                TargetType = sharedLink.TargetType,
                ExpirationDate = sharedLink.ExpirationDate,
                IsActive = sharedLink.IsActive,
                CreatedAt = sharedLink.CreatedAt,
                UpdatedAt = sharedLink.UpdatedAt,
                CanView = sharedLink.CanView,
                CanEdit = sharedLink.CanEdit,
                AllowDownload = sharedLink.AllowDownload
            };
        }
    }
}
