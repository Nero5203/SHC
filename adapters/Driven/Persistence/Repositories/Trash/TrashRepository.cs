using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Trash;
using Domain.Entities.Trash;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Trash
{
    public class TrashRepository : ITrashRepository
    {
        private readonly ShcDbContext _context;

        public TrashRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(TrashedItem trashedItem)
        {
            _context.TrashedItems.Add(trashedItem);
            await _context.SaveChangesAsync();
        }

        public async Task<TrashedItem?> GetByIdAsync(Guid trashedItemId)
        {
            return await _context.TrashedItems
                .FirstOrDefaultAsync(ti => ti.TrashedItemId == trashedItemId);
        }

        public async Task<IReadOnlyList<TrashedItem>> GetByUserIdAsync(Guid userId)
        {
            return await _context.TrashedItems
                .Where(ti => ti.UserId == userId && ti.RestoredAt == null)
                .OrderByDescending(ti => ti.DeletedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(TrashedItem trashedItem)
        {
            _context.TrashedItems.Update(trashedItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TrashedItem trashedItem)
        {
            _context.TrashedItems.Remove(trashedItem);
            await _context.SaveChangesAsync();
        }
    }
}
