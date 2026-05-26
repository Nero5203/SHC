namespace SHC.Domain.Entities.Permissions.Enums
{
    public enum AccessLevel
    {
        None = 0,
        Read = 1,       // Can view metadata or download files
        Write = 2,      // Can upload, modify, or create resources
        Delete = 3,     // Can permanently remove resources
        Admin = 4       // Full control: modify permissions, transfer ownership
    }
}