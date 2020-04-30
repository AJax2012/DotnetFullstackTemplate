using System;
using System.Collections.Generic;
using SourceName.Data.Model.Role;

namespace SourceName.Data.Model.User
{
    public class UserEntity : EntityWithIntegerId
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<UserRoleEntity> Roles { get; set; }
    }
}