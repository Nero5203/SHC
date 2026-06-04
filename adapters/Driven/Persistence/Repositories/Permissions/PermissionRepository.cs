using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Permissions;
using Microsoft.EntityFrameworkCore;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace adapters.Driven.Persistence.Repositories.Permissions
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ShcDbContext _context;

        public PermissionRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
        }

        public async Task<Permission?> GetByIdAsync(Guid permissionId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.PermissionId == permissionId);
        }

        public async Task<Permission?> GetForSubjectResourceAsync(
            SubjectType subjectType,
            Guid subjectId,
            ResourceType resourceType,
            Guid resourceId)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p =>
                    p.SubjectType == subjectType &&
                    p.SubjectId == subjectId &&
                    p.ResourceType == resourceType &&
                    p.ResourceId == resourceId);
        }

        public async Task<IReadOnlyList<Permission>> GetBySubjectAsync(SubjectType subjectType, Guid subjectId)
        {
            return await _context.Permissions
                .Where(p => p.SubjectType == subjectType && p.SubjectId == subjectId)
                .OrderByDescending(p => p.GrantedAtUtc)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Permission>> GetByResourceAsync(ResourceType resourceType, Guid resourceId)
        {
            return await _context.Permissions
                .Where(p => p.ResourceType == resourceType && p.ResourceId == resourceId)
                .OrderByDescending(p => p.GrantedAtUtc)
                .ToListAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Permission permission)
        {
            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();
        }
    }
}
