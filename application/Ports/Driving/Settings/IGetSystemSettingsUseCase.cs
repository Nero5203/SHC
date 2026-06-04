using Domain.Entities.Settings;

namespace application.Ports.Driving.Settings
{
    public interface IGetSystemSettingsUseCase
    {
        Task<SystemSetting> ExecuteAsync();
    }
}
