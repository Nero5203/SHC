using application.Ports.Driven.FileStorage;
using application.Ports.Driven.LinkSharing;
using application.Ports.Driving.FileStorage.Folder;
using Domain.Entities.LinkSharing;
using Domain.Entities.LinkSharing.Enums;
using domain.Entities.FileStorage.Enums;

namespace application.UseCases.FileStorage.Folder
{
    public class ShareFolderUseCase : IShareFolderUseCase
    {
        private readonly IFolderRepository _folderRepository;
        private readonly ISharedLinkRepository _sharedLinkRepository;

        public ShareFolderUseCase(
            IFolderRepository folderRepository,
            ISharedLinkRepository sharedLinkRepository)
        {
            _folderRepository = folderRepository;
            _sharedLinkRepository = sharedLinkRepository;
        }

        public async Task<SharedLink?> ExecuteAsync(Guid folderId, SharePermission permission, DateTime? expiresAt)
        {
            var folder = await _folderRepository.GetByIdAsync(folderId);

            if (folder == null || folder.IsDeleted)
            {
                return null;
            }

            var now = DateTime.UtcNow;
            var sharedLink = new SharedLink
            {
                SharedLinkId = Guid.NewGuid(),
                TokenUrl = Guid.NewGuid().ToString("N"),
                UserId = folder.UserId,
                TargetId = folder.FolderId,
                TargetType = ShareTargetType.Folder,
                ExpirationDate = expiresAt,
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
