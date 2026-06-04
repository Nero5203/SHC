namespace application.Dto.StorageNodes
{
    public class CreateStorageNodeDto
    {
        public string Name { get; set; } = null!;
        public string Hostname { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public int Port { get; set; }
        public string BasePath { get; set; } = null!;
        public long TotalCapacityBytes { get; set; }
    }
}
