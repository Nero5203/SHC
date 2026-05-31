namespace SHC.Domain.Entities.Permissions.Enums
{
    public enum SubjectType
    {
        User = 1,          // An actual human user account
        ServiceAccount = 2,// Automated background worker, CI/CD, or cron job
        StorageNode = 3,   // Node-to-node internal communication
        Guest = 4 ,        // Anonymous or publicly shared link access
        Role = 5 // Role-based permission assignment (Admin, User, Moderator, etc.)
    }
}