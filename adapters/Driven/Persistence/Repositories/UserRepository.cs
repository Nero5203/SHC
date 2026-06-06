using adapters.Driven.Persistence.Data;
using Domain.Entities.Auth;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using application.Ports.Driven;

namespace adapters.Driven.Persistence.Repositories
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

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User?> GetWithSettingsByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.UserSettings)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<UserCredential?> GetCredentialsByEmailAsync(string email)
        {
            return await _context.UserCredentials
                .Include(uc => uc.User)
                .FirstOrDefaultAsync(uc => uc.User.Email == email);
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
