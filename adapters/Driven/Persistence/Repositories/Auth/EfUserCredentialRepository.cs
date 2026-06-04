using Microsoft.EntityFrameworkCore;
using application.Ports.Driven.Auth;
using Domain.Entities.Auth;
using adapters.Driven.Persistence.Data;
using Domain.Entities.Users;

namespace adapters.Driven.Persistence.Repositories.Auth
{
    public class EfUserCredentialRepository : IUserCredentialRepository
    {
        private readonly ShcDbContext _context;

        public EfUserCredentialRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(UserCredential user)
        {
            _context.UserCredentials.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}