using Domain.Entities.Users;
using application.Ports.Driven;
using application.Ports.Driving;

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
