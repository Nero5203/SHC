using System;
using Domain.Entities.Users;

namespace Domain.Entities.Roles
{
    public class UserRole
    {
       
        public Guid UserId { get; set; }
        public virtual User User { get; set; } 

        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; } 

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}