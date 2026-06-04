using Domain.Entities.LinkSharing;

namespace application.Ports.Driving.LinkSharing
{
    public interface IUpdateSharedLinkUseCase
    {
        Task<SharedLink?> ExecuteAsync(
            Guid sharedLinkId,
            DateTime? expirationDate,
            bool? isActive,
            bool? canView,
            bool? canEdit,
            bool? allowDownload);
    }
}
