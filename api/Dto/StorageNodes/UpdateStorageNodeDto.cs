using Domain.Entities.StorageNodes.Enums;

namespace api.Dto.StorageNodes
{
    public class UpdateStorageNodeDto
    {
        public string? Name { get; set; }
        public string? Hostname { get; set; }
        public string? IpAddress { get; set; }
        public int? Port { get; set; }
        public string? BasePath { get; set; }
        public long? TotalCapacityBytes { get; set; }
        public NodeStatus? Status { get; set; }
    }
}
