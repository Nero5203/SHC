using Domain.Entities.LinkSharing;
using Domain.Entities.LinkSharing.Enums;

namespace application.Ports.Driving.LinkSharing
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
