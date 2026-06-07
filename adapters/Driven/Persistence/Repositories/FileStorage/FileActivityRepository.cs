using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using domain.Entities.FileStorage;
using domain.Entities.FileStorage.Enums;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class FileActivityRepository : IFileActivityRepository
    {
        private readonly ShcDbContext _context;

        public FileActivityRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task TrackAsync(Guid userId, Guid fileItemId, FileActivityType type)
        {
            var activity = new FileActivity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileItemId = fileItemId,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _context.FileActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FileActivity>> GetByUserInLastDaysAsync(Guid userId, int days)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);
            return await _context.FileActivities
                .Where(fa => fa.UserId == userId && fa.CreatedAt >= fromDate)
                .ToListAsync();
        }

        public async Task<List<FileActivity>> GetRecentByUserAsync(Guid userId, int take)
        {
            return await _context.FileActivities
                .Where(fa => fa.UserId == userId)
                .OrderByDescending(fa => fa.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}
