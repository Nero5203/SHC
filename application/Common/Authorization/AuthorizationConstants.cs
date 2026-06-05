namespace application.Common.Authorization
{
    public static class AuthorizationRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string StorageNodeOwner = "StorageNodeOwner";
    }

    public static class AuthorizationPermissions
    {
        public const string FileRead = "File.Read";
        public const string FileUpload = "File.Upload";
        public const string FileDelete = "File.Delete";
        public const string FileShare = "File.Share";
        public const string FolderCreate = "Folder.Create";
        public const string FolderDelete = "Folder.Delete";
        public const string FolderShare = "Folder.Share";
        public const string NodeRegister = "Node.Register";
        public const string NodeManage = "Node.Manage";
        public const string UserManage = "User.Manage";
        public const string SystemAdmin = "System.Admin";

        public static readonly IReadOnlyList<string> All = new[]
        {
            FileRead,
            FileUpload,
            FileDelete,
            FileShare,
            FolderCreate,
            FolderDelete,
            FolderShare,
            NodeRegister,
            NodeManage,
            UserManage,
            SystemAdmin
        };
    }

    public static class AuthorizationPolicies
    {
        public const string FileRead = "FileReadPolicy";
        public const string FileUpload = "FileUploadPolicy";
        public const string FileDelete = "FileDeletePolicy";
        public const string FileShare = "FileSharePolicy";
        public const string FolderManage = "FolderManagePolicy";
        public const string NodeManage = "NodeManagePolicy";
        public const string Admin = "AdminPolicy";
    }

    public static class ShcClaimTypes
    {
        public const string Permission = "permission";
    }
}
