using Domain.Entities.Users;
using ports.DrivenPorts;
using ports.DrivingPorts.Users;

namespace application.UseCases.Users
{
    public class GetUserByEmailUseCase : IGetUserByEmailUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserByEmailUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ExecuteAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
    }
}
