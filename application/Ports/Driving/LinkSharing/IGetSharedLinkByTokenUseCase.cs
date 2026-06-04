using Domain.Entities.LinkSharing;

namespace application.Ports.Driving.LinkSharing
{
    public interface IGetSharedLinkByTokenUseCase
    {
        Task<SharedLink?> ExecuteAsync(string tokenUrl);
    }
}
