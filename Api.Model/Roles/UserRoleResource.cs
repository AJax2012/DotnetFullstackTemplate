using System;

namespace SourceName.Api.Model.Roles
{
    public class UserRoleResource
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public RoleResource Role { get; set; }
    }
}