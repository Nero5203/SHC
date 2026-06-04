using Domain.Entities.LinkSharing;

namespace application.Ports.Driving.LinkSharing
{
    public interface IGetSharedLinkByIdUseCase
    {
        Task<SharedLink?> ExecuteAsync(Guid sharedLinkId);
    }
}
