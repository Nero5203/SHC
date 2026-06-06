using SHC.Domain.Entities.Permissions;

namespace application.Ports.Driving.Permissions
{
    public interface IGetAllPermissionsUseCase
    {
        Task<IReadOnlyList<Permission>> ExecuteAsync();
    }
}
