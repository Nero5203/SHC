using Domain.Entities.LinkSharing;

namespace application.Ports.Driven.LinkSharing
{
    public interface ISharedLinkRepository
    {
        Task CreateAsync(SharedLink sharedLink);
        Task<SharedLink?> GetByIdAsync(Guid sharedLinkId);
        Task<SharedLink?> GetByTokenUrlAsync(string tokenUrl);
        Task<IReadOnlyList<SharedLink>> GetByUserIdAsync(Guid userId);
        Task UpdateAsync(SharedLink sharedLink);
    }
}
