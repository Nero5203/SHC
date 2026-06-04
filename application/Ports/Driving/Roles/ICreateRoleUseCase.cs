using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface ICreateRoleUseCase
    {
        Task<Role> ExecuteAsync(string name, string description);
    }
}
