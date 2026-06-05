using Domain.Entities.LinkSharing;
using domain.Entities.FileStorage.Enums;

namespace application.Ports.Driving.FileStorage.Folder
{
    public interface IShareFolderUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid folderId, SharePermission permission, DateTime? expiresAt);
    }
}
