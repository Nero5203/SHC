using Domain.Entities.StorageNodes.Enums;

namespace api.Dto.StorageNodes
{
    public class StorageNodeResponseDto
    {
        public Guid StorageNodeId { get; set; }
        public string Name { get; set; } = null!;
        public string Hostname { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public int Port { get; set; }
        public string BasePath { get; set; } = null!;
        public long TotalCapacityBytes { get; set; }
        public long UsedCapacityBytes { get; set; }
        public NodeStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastHeartbeatAt { get; set; }
    }
}
