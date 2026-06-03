using Domain.Entities.LinkSharing;

namespace ports.DrivingPorts.LinkSharing
{
    public interface IGetSharedLinkByTokenUseCase
    {
        Task<SharedLink?> ExecuteAsync(string tokenUrl);
    }
}
