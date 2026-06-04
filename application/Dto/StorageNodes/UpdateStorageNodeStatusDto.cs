using Domain.Entities.StorageNodes.Enums;

namespace application.Dto.StorageNodes
{
    public class UpdateStorageNodeStatusDto
    {
        public NodeStatus Status { get; set; }
    }
}
