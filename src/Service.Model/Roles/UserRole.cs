using System;

namespace SourceName.Service.Model.Roles
{
    public class UserRole
    {
        int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}