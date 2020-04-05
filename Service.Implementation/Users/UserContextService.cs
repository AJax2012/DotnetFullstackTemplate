using System;
using System.Collections.Generic;
using System.Linq;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserContextService : IUserContextService
    {
        private Guid? _userId;

        public Guid? UserId
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
                ? Guid.Parse(userId)
                : (Guid?)null;
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