using System;
using System.Collections.Generic;

namespace Domain.Entities.Authorization
{
    public class Permission
    {
        public Guid PermissionId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
