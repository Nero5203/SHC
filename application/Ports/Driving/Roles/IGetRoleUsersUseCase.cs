using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IGetRoleUsersUseCase
    {
        Task<IReadOnlyList<UserRole>> ExecuteAsync(Guid roleId);
    }
}
