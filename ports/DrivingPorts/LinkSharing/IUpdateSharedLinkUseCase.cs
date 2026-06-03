using Domain.Entities.LinkSharing;

namespace ports.DrivingPorts.LinkSharing
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
