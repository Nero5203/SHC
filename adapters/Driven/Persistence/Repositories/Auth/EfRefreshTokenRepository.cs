using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Auth;
using Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Auth
{
    public class EfRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ShcDbContext  _context;

        public EfRefreshTokenRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }
    }
}