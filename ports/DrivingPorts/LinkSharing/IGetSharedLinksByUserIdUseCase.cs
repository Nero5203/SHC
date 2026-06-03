using Domain.Entities.LinkSharing;

namespace ports.DrivingPorts.LinkSharing
{
    public interface IGetSharedLinksByUserIdUseCase
    {
        Task<IReadOnlyList<SharedLink>> ExecuteAsync(Guid userId);
    }
}
