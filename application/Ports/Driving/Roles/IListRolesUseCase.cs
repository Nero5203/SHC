using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IListRolesUseCase
    {
        Task<IReadOnlyList<Role>> ExecuteAsync();
    }
}
