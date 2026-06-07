using adapters.Driven.Persistence.Data;
using application.Ports.Driven.AI;
using Domain.Entities.AI;
using Domain.Entities.AI.Enums;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.AI
{
    public class AISuggestionRepository : IAISuggestionRepository
    {
        private readonly ShcDbContext _context;
        public AISuggestionRepository(ShcDbContext context){
            _context = context;
        }
        public async Task AddRangeAsync(List<AISuggestion> suggestions)
        {
            if (suggestions.Count == 0)
            {
                return;
            }

            await _context.AISuggestions.AddRangeAsync(suggestions);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid fileItemId, AISuggestionType suggestionType)
        {
            return await _context.AISuggestions.AnyAsync(s =>
                s.UserId == userId &&
                s.FileItemId == fileItemId &&
                s.Type == suggestionType &&
                s.Status == AISuggestionStatus.Pending);
        }
        public async Task<IReadOnlyList<AISuggestion>> GetPendingByUserAsync(Guid userId)
        {
            return await _context.AISuggestions
                .Where(s => s.UserId == userId && s.Status == AISuggestionStatus.Pending)
                .Include(s => s.FileItem)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<AISuggestion?> GetByIdAsync(Guid suggestionId)
        {
            return await _context.AISuggestions
                .Include(s => s.FileItem)
                .FirstOrDefaultAsync(s => s.Id == suggestionId);
        }

        public async Task UpdateAsync(AISuggestion suggestion)
        {
            _context.AISuggestions.Update(suggestion);
            await _context.SaveChangesAsync();
        }
    }
}
