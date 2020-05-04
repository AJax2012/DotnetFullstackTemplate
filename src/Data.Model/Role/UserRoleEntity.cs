using System;
using SourceName.Data.Model.User;

namespace SourceName.Data.Model.Role
{
    public class UserRoleEntity : EntityWithIntegerId
    {
        public int? UserId { get; set; }
        public int RoleId { get; set; }

        public virtual UserEntity User { get; set; }
        public virtual RoleEntity Role { get; set; }
    }
}