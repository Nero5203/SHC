using application.Ports.Driven.Permissions;
using application.Ports.Driving.Permissions;
using SHC.Domain.Entities.Permissions;
using SHC.Domain.Entities.Permissions.Enums;

namespace application.UseCases.Permissions
{
    public class GetPermissionsBySubjectUseCase : IGetPermissionsBySubjectUseCase
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsBySubjectUseCase(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<IReadOnlyList<Permission>> ExecuteAsync(SubjectType subjectType, Guid subjectId)
        {
            return await _permissionRepository.GetBySubjectAsync(subjectType, subjectId);
        }
    }
}
