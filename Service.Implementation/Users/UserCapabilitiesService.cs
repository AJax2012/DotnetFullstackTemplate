using System;
using System.Collections.Generic;
using System.Linq;
using SourceName.Service.Model.Roles;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserCapabilitiesService : IUserCapabilitiesService
    {
        private static readonly List<Roles> RolesCanManageUsers = new List<Roles>
        {
            Roles.Administrator
        };

        private readonly IUserService _userService;

        public UserCapabilitiesService(IUserService userService)
        {
            _userService = userService;
        }

        public UserCapabilities GetUserCapabilities(int userId)
        {
            var user = _userService.GetById(userId);
            return new UserCapabilities
            {
                CanManageUsers = GetCanManageUsers(user)
            };
        }

        private bool GetCanManageUsers(User user)
        {
            return user.Roles.Any(r =>
                RolesCanManageUsers
                    .Select(r => (int)r)
                    .Contains(r.RoleId));
        }
    }
}