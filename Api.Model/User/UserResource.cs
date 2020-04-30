using System;
using System.Collections.Generic;
using SourceName.Api.Model.Roles;

namespace SourceName.Api.Model.User
{
    public class UserResource
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public List<UserRoleResource> Roles { get; set; }
        public bool IsActive { get; set; }
    }
}