using Domain.Entities.LinkSharing;

namespace application.Ports.Driving.LinkSharing
{
    public interface IGetSharedLinksByUserIdUseCase
    {
        Task<IReadOnlyList<SharedLink>> ExecuteAsync(Guid userId);
    }
}
