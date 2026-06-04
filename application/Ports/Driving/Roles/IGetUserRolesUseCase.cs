using Domain.Entities.Roles;

namespace application.Ports.Driving.Roles
{
    public interface IGetUserRolesUseCase
    {
        Task<IReadOnlyList<UserRole>> ExecuteAsync(Guid userId);
    }
}
