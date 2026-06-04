using Domain.Entities.LinkSharing;

namespace application.Ports.Driving.LinkSharing
{
    public interface IDeactivateSharedLinkUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid sharedLinkId);
    }
}
