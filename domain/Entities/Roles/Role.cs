using System;
using System.Collections.Generic;
using Domain.Entities.Users; // Assuming User entity is here

namespace Domain.Entities.Roles
{
    public class Role
    {
        public Guid RoleId { get; set; } = Guid.NewGuid();

        
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}