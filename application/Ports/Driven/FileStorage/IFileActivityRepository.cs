using domain.Entities.FileStorage;

namespace application.Ports.Driven.FileStorage
{
    public interface IFileActivityRepository
    {
        Task<List<FileActivity>> GetRecentByUserAsync(Guid userId, int take);
        Task<List<FileActivity>> GetByUserInLastDaysAsync(Guid userId, int days);
    }
}