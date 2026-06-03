using Domain.Entities.LinkSharing;
using Domain.Entities.LinkSharing.Enums;

namespace ports.DrivingPorts.LinkSharing
{
    public interface ICreateSharedLinkUseCase
    {
        Task<SharedLink> ExecuteAsync(
            Guid userId,
            Guid targetId,
            ShareTargetType targetType,
            DateTime? expirationDate,
            bool canView,
            bool canEdit,
            bool allowDownload);
    }
}
