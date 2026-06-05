using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Roles;
using Domain.Entities.Roles;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Roles
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ShcDbContext _context;

        public RoleRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.Users
                .AnyAsync(u => u.UserId == userId);
        }

        public async Task<UserRole?> GetUserRoleAsync(Guid userId, Guid roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        }

        public async Task AssignRoleToUserAsync(UserRole userRole)
        {
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUserAsync(UserRole userRole)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<UserRole>> GetUserRolesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .OrderBy(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserRole>> GetUserRolesWithPermissionsAsync(Guid userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId)
                .OrderBy(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetUserPermissionNamesAsync(Guid userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))
                .Where(name => name != string.Empty)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<UserRole>> GetRoleUsersAsync(Guid roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId)
                .OrderBy(ur => ur.User.Username)
                .ToListAsync();
        }
    }
}
