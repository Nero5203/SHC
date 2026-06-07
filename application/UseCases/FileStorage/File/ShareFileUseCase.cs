using application.Ports.Driven.FileStorage;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.FileStorage.File;
using Domain.Entities.LinkSharing;
using Domain.Entities.LinkSharing.Enums;
using domain.Entities.FileStorage.Enums;

namespace application.UseCases.FileStorage.File
{
    public class ShareFileUseCase : IShareFileUseCase
    {
        private readonly IFileRepository _fileRepository;
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public ShareFileUseCase(
            IFileRepository fileRepository,
            ISharedLinkRepository sharedLinkRepository)
        {
            _fileRepository = fileRepository;
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(Guid fileItemId, SharePermission permission, DateTime? expiresAt)
        {
            var fileItem = await _fileRepository.GetByIdAsync(fileItemId);

            if (fileItem == null || fileItem.IsDeleted)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var sharedLink = new SharedLink
            {
                SharedLinkId = Guid.NewGuid(),
                TokenUrl = Guid.NewGuid().ToString("N"),
                UserId = fileItem.UserId,
                TargetId = fileItem.FileItemId,
                TargetType = ShareTargetType.File,
                ExpirationDate = expiresAt ?? DateTime.UtcNow.AddDays(7),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CanView = true,
                CanEdit = permission == SharePermission.Edit,
                AllowDownload = permission == SharePermission.Download || permission == SharePermission.Edit
            };

            await _sharedLinkRepository.CreateAsync(sharedLink);

            return sharedLink;
        }
    }
}
