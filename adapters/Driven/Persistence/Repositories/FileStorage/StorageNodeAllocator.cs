using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class StorageNodeAllocator : IStorageNodeAllocator
    {
        private readonly ShcDbContext _context;

        public StorageNodeAllocator(ShcDbContext context)
        {
            _context = context;
        }

        public async Task<StorageNode?> GetBestAvailableNodeAsync(long requiredBytes)
        {
            return await _context.StorageNodes
                .Where(sn =>
                    sn.Status == NodeStatus.Online &&
                    sn.TotalCapacityBytes >= sn.UsedCapacityBytes + requiredBytes)
                .OrderByDescending(sn => sn.TotalCapacityBytes - sn.UsedCapacityBytes)
                .FirstOrDefaultAsync();
        }
    }
}
