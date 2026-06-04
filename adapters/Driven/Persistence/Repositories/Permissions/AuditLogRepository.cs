using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Permissions;
using Microsoft.EntityFrameworkCore;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace adapters.Driven.Persistence.Repositories.Permissions
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ShcDbContext _context;

        public AuditLogRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .OrderByDescending(al => al.TimestampUtc)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetBySubjectAsync(SubjectType subjectType, Guid subjectId)
        {
            return await _context.AuditLogs
                .Where(al => al.SubjectType == subjectType && al.SubjectId == subjectId)
                .OrderByDescending(al => al.TimestampUtc)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<AuditLog>> GetByResourceAsync(ResourceType resourceType, Guid resourceId)
        {
            var resourceIdValue = resourceId.ToString();

            return await _context.AuditLogs
                .Where(al => al.ResourceType == resourceType && al.ResourceId == resourceIdValue)
                .OrderByDescending(al => al.TimestampUtc)
                .ToListAsync();
        }
    }
}
