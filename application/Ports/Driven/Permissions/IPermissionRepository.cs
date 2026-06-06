using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driven.Permissions
{
    public interface IPermissionRepository
    {
        Task CreateAsync(Permission permission);
        Task<Permission?> GetByIdAsync(Guid permissionId);
        Task<IReadOnlyList<Permission>> GetAllAsync();
        Task<Permission?> GetForSubjectResourceAsync(
            SubjectType subjectType,
            Guid subjectId,
            ResourceType resourceType,
            Guid resourceId);
        Task<IReadOnlyList<Permission>> GetBySubjectAsync(SubjectType subjectType, Guid subjectId);
        Task<IReadOnlyList<Permission>> GetByResourceAsync(ResourceType resourceType, Guid resourceId);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(Permission permission);
    }
}
