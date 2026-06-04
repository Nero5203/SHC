using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IAssignRoleToUserUseCase
    {
        Task<UserRole?> ExecuteAsync(Guid roleId, Guid userId);
    }
}
