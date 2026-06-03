using Domain.Entities.LinkSharing;

namespace ports.DrivingPorts.LinkSharing
{
    public interface IDeactivateSharedLinkUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid sharedLinkId);
    }
}
