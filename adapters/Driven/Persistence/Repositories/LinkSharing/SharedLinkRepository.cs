using adapters.Driven.Persistence.Data;
using Domain.Entities.LinkSharing;
using Microsoft.EntityFrameworkCore;
using application.Ports.Driven.LinkSharing;

namespace adapters.Driven.Persistence.Repositories.LinkSharing
{
    public class SharedLinkRepository : ISharedLinkRepository
    {
        private readonly ShcDbContext _context;

        public SharedLinkRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(SharedLink sharedLink)
        {
            _context.SharedLinks.Add(sharedLink);
            await _context.SaveChangesAsync();
        }

        public async Task<SharedLink?> GetByIdAsync(Guid sharedLinkId)
        {
            return await _context.SharedLinks
                .FirstOrDefaultAsync(sl => sl.SharedLinkId == sharedLinkId);
        }

        public async Task<SharedLink?> GetByTokenUrlAsync(string tokenUrl)
        {
            return await _context.SharedLinks
                .FirstOrDefaultAsync(sl => sl.TokenUrl == tokenUrl);
        }

        public async Task<IReadOnlyList<SharedLink>> GetByUserIdAsync(Guid userId)
        {
            return await _context.SharedLinks
                .Where(sl => sl.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateAsync(SharedLink sharedLink)
        {
            _context.SharedLinks.Update(sharedLink);
            await _context.SaveChangesAsync();
        }
    }
}
