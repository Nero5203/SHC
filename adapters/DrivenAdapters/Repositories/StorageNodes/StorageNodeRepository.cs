using adapters.DrivenAdapters.Data;
using Domain.Entities.StorageNodes;
using Domain.Entities.StorageNodes.Enums;
using Microsoft.EntityFrameworkCore;
using ports.DrivenPorts.StorageNodes;

namespace adapters.DrivenAdapters.Repositories.StorageNodes
{
    public class StorageNodeRepository : IStorageNodeRepository
    {
        private readonly ShcDbContext _context;

        public StorageNodeRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(StorageNode storageNode)
        {
            _context.StorageNodes.Add(storageNode);
            await _context.SaveChangesAsync();
        }

        public async Task<StorageNode?> GetByIdAsync(Guid storageNodeId)
        {
            return await _context.StorageNodes
                .Include(sn => sn.FileItems)
                .FirstOrDefaultAsync(sn => sn.StorageNodeId == storageNodeId);
        }

        public async Task<IReadOnlyList<StorageNode>> GetAllAsync()
        {
            return await _context.StorageNodes
                .Include(sn => sn.FileItems)
                .ToListAsync();
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

        public async Task UpdateAsync(StorageNode storageNode)
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHeartbeatAsync(
            StorageNode storageNode,
            long totalCapacityBytes,
            long usedCapacityBytes,
            NodeStatus status)
        {
            storageNode.TotalCapacityBytes = totalCapacityBytes;
            storageNode.UsedCapacityBytes = usedCapacityBytes;
            storageNode.Status = status;
            storageNode.LastHeartbeatAt = DateTime.UtcNow;
            storageNode.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(StorageNode storageNode, NodeStatus status)
        {
            storageNode.Status = status;
            storageNode.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
