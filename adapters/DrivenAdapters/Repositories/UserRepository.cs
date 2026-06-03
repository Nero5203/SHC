using adapters.DrivenAdapters.Data;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using ports.DrivenPorts;

namespace adapters.DrivenAdapters.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ShcDbContext _context;

        public UserRepository(ShcDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetWithSettingsByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.UserSettings)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }
    }
}