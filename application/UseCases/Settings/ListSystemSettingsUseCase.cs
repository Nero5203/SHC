using application.Ports.Driven.Settings;
using application.Ports.Driving.Settings;
using Domain.Entities.Settings;

namespace application.UseCases.Settings
{
    public class ListSystemSettingsUseCase : IListSystemSettingsUseCase
    {
        private readonly ISystemSettingRepository _systemSettingRepository;

        public ListSystemSettingsUseCase(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public async Task<IReadOnlyList<SystemSetting>> ExecuteAsync()
        {
            var systemSettings = await _systemSettingRepository.GetAllAsync();

            if (systemSettings.Count > 0)
            {
                return systemSettings;
            }

            var defaultSystemSetting = await _systemSettingRepository.GetAsync();

            return new List<SystemSetting> { defaultSystemSetting };
        }
    }
}
