using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.Ports.Driving.Permissions
{
    public interface IGetPermissionsBySubjectUseCase
    {
        Task<IReadOnlyList<Permission>> ExecuteAsync(SubjectType subjectType, Guid subjectId);
    }
}
