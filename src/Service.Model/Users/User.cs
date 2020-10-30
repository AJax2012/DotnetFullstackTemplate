using System;
using System.Collections.Generic;
using SourceName.Service.Model.Roles;

namespace SourceName.Service.Model.Users
{
    public class User
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool IsActive { get; set; }
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}