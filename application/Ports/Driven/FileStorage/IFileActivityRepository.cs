using domain.Entities.FileStorage;
using domain.Entities.FileStorage.Enums;

namespace application.Ports.Driven.FileStorage
{
    public interface IFileActivityRepository
    {
        Task TrackAsync(Guid userId, Guid fileItemId, FileActivityType type);
        Task<List<FileActivity>> GetRecentByUserAsync(Guid userId, int take);
        Task<List<FileActivity>> GetByUserInLastDaysAsync(Guid userId, int days);
    }
}
