using Domain.Entities.Users;

namespace application.Ports.Driving
{
    public interface IGetAllUsersUseCase
    {
        Task<IReadOnlyList<User>> ExecuteAsync();
    }
}
