using System;
using System.Collections.Generic;
using System.Linq;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserContextService : IUserContextService
    {
        private int? _userId;

        public int? UserId
        {
            get
            {
                return _userId;
            }
        }

        public List<int> RoleIds { get; set; } = new List<int>();

        public void SetCurrentUserId(string userId)
        {
            _userId = !string.IsNullOrWhiteSpace(userId)
                ? int.Parse(userId)
                : (int?)null;
        }

        public void SetUserRoleIds(string roleIds)
        {
            RoleIds.Clear();
            if (!string.IsNullOrWhiteSpace(roleIds))
            {
                RoleIds.AddRange(roleIds.Split(",").Select(int.Parse));
            }
        }
    }
}