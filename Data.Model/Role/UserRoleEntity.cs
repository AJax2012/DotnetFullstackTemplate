using System;
using SourceName.Data.Model.User;

namespace SourceName.Data.Model.Role
{
    public class UserRoleEntity : EntityWithIntegerId
    {
        public Guid ApplicationUserId { get; set; }
        public int ApplicationRoleId { get; set; }

        public virtual UserEntity User { get; set; }
        public virtual RoleEntity Role { get; set; }
    }
}