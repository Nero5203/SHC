using adapters.Driven.Persistence.Data;
using application.Ports.Driven.FileStorage;
using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.FileStorage
{
    public class StorageNodeAllocator : IStorageNodeAllocator
    {
        private static int _nextNodeIndex = -1;
        private readonly ShcDbContext _context;

        public StorageNodeAllocator(ShcDbContext context)
        {
            _context = context;
        }

        public async Task<StorageNode?> GetBestAvailableNodeAsync(long requiredBytes)
        {
            var availableNodes = await _context.StorageNodes
                .Where(sn =>
                    sn.Status == NodeStatus.Online &&
                    sn.TotalCapacityBytes >= sn.UsedCapacityBytes + requiredBytes)
                .OrderBy(sn => sn.CreatedAt)
                .ThenBy(sn => sn.StorageNodeId)
                .ToListAsync();

            if (availableNodes.Count == 0)
            {
                return null;
            }

            var selectedIndex = Interlocked.Increment(ref _nextNodeIndex) % availableNodes.Count;

            return availableNodes[selectedIndex];
        }
    }
}
