using Domain.Entities.Settings;

namespace application.Ports.Driven.Settings
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting> GetAsync();
        Task<IReadOnlyList<SystemSetting>> GetAllAsync();
        Task UpdateAsync(SystemSetting systemSetting);
    }
}
