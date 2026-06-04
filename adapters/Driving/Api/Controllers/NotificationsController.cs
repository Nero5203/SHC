using application.Dto.Notifications;
using application.Ports.Driving.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ICreateNotificationUseCase _createNotificationUseCase;
        private readonly IGetNotificationByIdUseCase _getNotificationByIdUseCase;
        private readonly IGetNotificationsByUserIdUseCase _getNotificationsByUserIdUseCase;
        private readonly IGetUnreadNotificationsByUserIdUseCase _getUnreadNotificationsByUserIdUseCase;
        private readonly IMarkNotificationAsReadUseCase _markNotificationAsReadUseCase;
        private readonly IMarkAllNotificationsAsReadUseCase _markAllNotificationsAsReadUseCase;
        private readonly IDeleteNotificationUseCase _deleteNotificationUseCase;

        public NotificationsController(
            ICreateNotificationUseCase createNotificationUseCase,
            IGetNotificationByIdUseCase getNotificationByIdUseCase,
            IGetNotificationsByUserIdUseCase getNotificationsByUserIdUseCase,
            IGetUnreadNotificationsByUserIdUseCase getUnreadNotificationsByUserIdUseCase,
            IMarkNotificationAsReadUseCase markNotificationAsReadUseCase,
            IMarkAllNotificationsAsReadUseCase markAllNotificationsAsReadUseCase,
            IDeleteNotificationUseCase deleteNotificationUseCase)
        {
            _createNotificationUseCase = createNotificationUseCase;
            _getNotificationByIdUseCase = getNotificationByIdUseCase;
            _getNotificationsByUserIdUseCase = getNotificationsByUserIdUseCase;
            _getUnreadNotificationsByUserIdUseCase = getUnreadNotificationsByUserIdUseCase;
            _markNotificationAsReadUseCase = markNotificationAsReadUseCase;
            _markAllNotificationsAsReadUseCase = markAllNotificationsAsReadUseCase;
            _deleteNotificationUseCase = deleteNotificationUseCase;
        }

        [HttpPost]
        public async Task<ActionResult<NotificationResponseDto>> CreateNotification(CreateNotificationDto dto)
        {
            try
            {
                var notification = await _createNotificationUseCase.ExecuteAsync(
                    dto.Title,
                    dto.Message,
                    dto.Type,
                    dto.Channel,
                    dto.UserId,
                    dto.TriggeredById,
                    dto.FileId,
                    dto.FolderId);

                return CreatedAtAction(
                    nameof(GetNotificationById),
                    new { notificationId = notification.NotificationId },
                    MapNotification(notification));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("{notificationId:guid}")]
        public async Task<ActionResult<NotificationResponseDto>> GetNotificationById(Guid notificationId)
        {
            var notification = await _getNotificationByIdUseCase.ExecuteAsync(notificationId);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(MapNotification(notification));
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IReadOnlyList<NotificationResponseDto>>> GetNotificationsByUserId(Guid userId)
        {
            var notifications = await _getNotificationsByUserIdUseCase.ExecuteAsync(userId);

            var response = notifications
                .Select(MapNotification)
                .ToList();

            return Ok(response);
        }

        [HttpGet("user/{userId:guid}/unread")]
        public async Task<ActionResult<IReadOnlyList<NotificationResponseDto>>> GetUnreadNotificationsByUserId(Guid userId)
        {
            var notifications = await _getUnreadNotificationsByUserIdUseCase.ExecuteAsync(userId);

            var response = notifications
                .Select(MapNotification)
                .ToList();

            return Ok(response);
        }

        [HttpPut("{notificationId:guid}/read")]
        public async Task<ActionResult<NotificationResponseDto>> MarkNotificationAsRead(Guid notificationId)
        {
            var notification = await _markNotificationAsReadUseCase.ExecuteAsync(notificationId);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(MapNotification(notification));
        }

        [HttpPut("user/{userId:guid}/read-all")]
        public async Task<ActionResult<int>> MarkAllNotificationsAsRead(Guid userId)
        {
            var updatedCount = await _markAllNotificationsAsReadUseCase.ExecuteAsync(userId);

            return Ok(updatedCount);
        }

        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var deleted = await _deleteNotificationUseCase.ExecuteAsync(notificationId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static NotificationResponseDto MapNotification(global::Notification notification)
        {
            return new NotificationResponseDto
            {
                NotificationId = notification.NotificationId,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead,
                Type = notification.Type,
                Channel = notification.Channel,
                UserId = notification.UserId,
                TriggeredById = notification.TriggeredById,
                FileId = notification.FileId,
                FolderId = notification.FolderId
            };
        }
    }
}
