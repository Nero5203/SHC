using Domain.Entities.LinkSharing;
using Domain.Entities.LinkSharing.Enums;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.LinkSharing;

namespace application.UseCases.LinkSharing
{
    public class CreateSharedLinkUseCase : ICreateSharedLinkUseCase
    {
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public CreateSharedLinkUseCase(ISharedLinkRepository sharedLinkRepository)
        {
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink> ExecuteAsync(
            Guid userId,
            Guid targetId,
            ShareTargetType targetType,
            DateTime? expirationDate,
            bool canView,
            bool canEdit,
            bool allowDownload)
        {
            var now = DateTime.UtcNow;

            var sharedLink = new SharedLink
            {
                SharedLinkId = Guid.NewGuid(),
                TokenUrl = Guid.NewGuid().ToString("N"),
                UserId = userId,
                TargetId = targetId,
                TargetType = targetType,
                ExpirationDate = expirationDate,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CanView = canView,
                CanEdit = canEdit,
                AllowDownload = allowDownload
            };

            await _sharedLinkRepository.CreateAsync(sharedLink);

            return sharedLink;
        }
    }
}
