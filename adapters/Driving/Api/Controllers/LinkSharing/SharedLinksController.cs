using api.Auditing;
using application.Dto.LinkSharing;
using Domain.Entities.LinkSharing;
using Microsoft.AspNetCore.Mvc;
using application.Ports.Driving.LinkSharing;
using application.Ports.Driving.Notifications;
using Domain.Entities.LinkSharing.Enums;
using Domain.Entities.Notifications.Enums;
using SHC.Domain.Entities.Permissions.Enums;

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
        private readonly ICreateNotificationUseCase _createNotificationUseCase;
        private readonly IAuditLogWriter _auditLogWriter;

        public SharedLinksController(
            ICreateSharedLinkUseCase createSharedLinkUseCase,
            IGetSharedLinkByIdUseCase getSharedLinkByIdUseCase,
            IGetSharedLinkByTokenUseCase getSharedLinkByTokenUseCase,
            IGetSharedLinksByUserIdUseCase getSharedLinksByUserIdUseCase,
            IUpdateSharedLinkUseCase updateSharedLinkUseCase,
            IDeactivateSharedLinkUseCase deactivateSharedLinkUseCase,
            ICreateNotificationUseCase createNotificationUseCase,
            IAuditLogWriter auditLogWriter)
        {
            _createSharedLinkUseCase = createSharedLinkUseCase;
            _getSharedLinkByIdUseCase = getSharedLinkByIdUseCase;
            _getSharedLinkByTokenUseCase = getSharedLinkByTokenUseCase;
            _getSharedLinksByUserIdUseCase = getSharedLinksByUserIdUseCase;
            _updateSharedLinkUseCase = updateSharedLinkUseCase;
            _deactivateSharedLinkUseCase = deactivateSharedLinkUseCase;
            _createNotificationUseCase = createNotificationUseCase;
            _auditLogWriter = auditLogWriter;
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
            await CreateSharedLinkNotificationAsync(sharedLink);
            await WriteSharedLinkAuditAsync("SharedLink.Created", sharedLink);

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

            await WriteSharedLinkAuditAsync("SharedLink.Updated", sharedLink);

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

            await WriteSharedLinkAuditAsync("SharedLink.Deactivated", sharedLink);

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

        private async Task CreateSharedLinkNotificationAsync(SharedLink sharedLink)
        {
            var isFolder = sharedLink.TargetType == ShareTargetType.Folder;

            await _createNotificationUseCase.ExecuteAsync(
                isFolder ? "Folder shared" : "File shared",
                isFolder
                    ? "A share link was created for one of your folders."
                    : "A share link was created for one of your files.",
                isFolder ? NotificationType.FolderShared : NotificationType.FileShared,
                NotificationChannel.InApp,
                sharedLink.UserId,
                sharedLink.UserId,
                isFolder ? null : sharedLink.TargetId,
                isFolder ? sharedLink.TargetId : null);
        }

        private async Task WriteSharedLinkAuditAsync(string action, SharedLink sharedLink)
        {
            await _auditLogWriter.WriteAsync(
                User,
                action,
                ResourceType.SharedLink,
                sharedLink.SharedLinkId.ToString(),
                true,
                new
                {
                    sharedLink.SharedLinkId,
                    sharedLink.UserId,
                    sharedLink.TargetId,
                    sharedLink.TargetType,
                    sharedLink.IsActive,
                    sharedLink.CanView,
                    sharedLink.CanEdit,
                    sharedLink.AllowDownload,
                    sharedLink.ExpirationDate
                });
        }
    }
}
