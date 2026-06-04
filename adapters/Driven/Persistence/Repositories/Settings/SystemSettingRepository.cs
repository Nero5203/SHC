using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Settings;
using Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Settings
{
    public class SystemSettingRepository : ISystemSettingRepository
    {
        private readonly ShcDbContext _context;

        public SystemSettingRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSetting> GetAsync()
        {
            var systemSetting = await _context.SystemSettings
                .OrderBy(ss => ss.CreatedAt)
                .FirstOrDefaultAsync();

            if (systemSetting != null)
            {
                return systemSetting;
            }

            systemSetting = new SystemSetting();

            _context.SystemSettings.Add(systemSetting);
            await _context.SaveChangesAsync();

            return systemSetting;
        }

        public async Task<IReadOnlyList<SystemSetting>> GetAllAsync()
        {
            return await _context.SystemSettings
                .OrderBy(ss => ss.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(SystemSetting systemSetting)
        {
            _context.SystemSettings.Update(systemSetting);
            await _context.SaveChangesAsync();
        }
    }
}
