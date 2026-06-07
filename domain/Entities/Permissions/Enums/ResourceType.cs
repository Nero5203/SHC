namespace SHC.Domain.Entities.Permissions.Enums
{
    public enum ResourceType
    {
        File = 1,
        Folder = 2,
        Bucket = 3,         // Logical grouping of files/folders
        StorageNode = 4,    // Physical/virtual hardware nodes
        Permission = 5,     // Protecting security rules themselves
        SystemSettings = 6, // Global cloud configuration
        Purchase = 7,
        Invoice = 8,
        Subscription = 9,
        SharedLink = 10,
        Role = 11
    }
}
