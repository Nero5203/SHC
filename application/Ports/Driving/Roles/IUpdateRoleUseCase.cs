using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IUpdateRoleUseCase
    {
        Task<Role?> ExecuteAsync(Guid roleId, string? name, string? description);
    }
}
