using Domain.Entities.Settings;

namespace application.Ports.Driving.Settings
{
    public interface IListSystemSettingsUseCase
    {
        Task<IReadOnlyList<SystemSetting>> ExecuteAsync();
    }
}
