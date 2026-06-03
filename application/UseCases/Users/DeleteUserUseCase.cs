using ports.DrivenPorts;
using ports.DrivingPorts;

namespace application.UseCases.Users
{
    public class DeleteUserUseCase : IDeleteUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(user);

            return true;
        }
    }
}
