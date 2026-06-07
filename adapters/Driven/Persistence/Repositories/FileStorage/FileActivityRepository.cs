using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using domain.Entities.FileStorage;
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