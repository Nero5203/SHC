using Domain.Entities.Users;
using ports.DrivenPorts;
using ports.DrivingPorts;

namespace application.UseCases.Users
{
    public class GetUserSettingsUseCase : IGetUserSettingsUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserSettingsUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ExecuteAsync(Guid userId)
        {
            return await _userRepository.GetWithSettingsByIdAsync(userId);
        }
    }
}
