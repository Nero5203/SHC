using application.Ports.Driven.Settings;
using application.Ports.Driving.Settings;
using Domain.Entities.Settings;

namespace application.UseCases.Settings
{
    public class GetSystemSettingsUseCase : IGetSystemSettingsUseCase
    {
        private readonly ISystemSettingRepository _systemSettingRepository;

        public GetSystemSettingsUseCase(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public async Task<SystemSetting> ExecuteAsync()
        {
            return await _systemSettingRepository.GetAsync();
        }
    }
}
