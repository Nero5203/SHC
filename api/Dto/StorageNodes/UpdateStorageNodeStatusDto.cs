using Domain.Entities.StorageNodes.Enums;

namespace api.Dto.StorageNodes
{
    public class UpdateStorageNodeStatusDto
    {
        public NodeStatus Status { get; set; }
    }
}
