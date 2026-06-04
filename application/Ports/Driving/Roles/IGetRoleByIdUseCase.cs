using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IGetRoleByIdUseCase
    {
        Task<Role?> ExecuteAsync(Guid roleId);
    }
}
