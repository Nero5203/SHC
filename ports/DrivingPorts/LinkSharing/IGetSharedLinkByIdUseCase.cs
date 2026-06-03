using Domain.Entities.LinkSharing;

namespace ports.DrivingPorts.LinkSharing
{
    public interface IGetSharedLinkByIdUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid sharedLinkId);
    }
}
