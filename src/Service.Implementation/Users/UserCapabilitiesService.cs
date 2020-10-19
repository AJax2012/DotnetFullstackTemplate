using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<UserCapabilities> GetUserCapabilitiesAsync(int userId)
        {
            var user = await _userService.GetByIdWithRolesAsync(userId);
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