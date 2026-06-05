using Domain.Entities.LinkSharing;
using domain.Entities.FileStorage.Enums;

namespace application.Ports.Driving.FileStorage.File
{
    public interface IShareFileUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid fileItemId, SharePermission permission, DateTime? expiresAt);
    }
}
