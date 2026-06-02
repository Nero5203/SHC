using Domain.Entities.StorageNodes.Enums;

namespace api.Dto.StorageNodes
{
    public class StorageNodeHeartbeatDto
    {
        public long UsedCapacityBytes { get; set; }
        public NodeStatus Status { get; set; }
    }
}
