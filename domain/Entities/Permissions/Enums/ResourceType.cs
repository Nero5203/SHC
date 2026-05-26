namespace SHC.Domain.Entities.Permissions.Enums
{
    public enum ResourceType
    {
        File = 1,
        Folder = 2,
        Bucket = 3,         // Logical grouping of files/folders
        StorageNode = 4,    // Physical/virtual hardware nodes
        Permission = 5,     // Protecting security rules themselves
        SystemSettings = 6  // Global cloud configuration
    }
}