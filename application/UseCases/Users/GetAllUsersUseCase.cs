using application.Ports.Driven;
using application.Ports.Driving;
using Domain.Entities.Users;

namespace application.UseCases.Users
{
    public class GetAllUsersUseCase : IGetAllUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyList<User>> ExecuteAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }
}
