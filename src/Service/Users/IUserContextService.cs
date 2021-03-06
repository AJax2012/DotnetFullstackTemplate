﻿using System;
using System.Collections.Generic;

namespace SourceName.Service.Users
{
    public interface IUserContextService
    {
        int? UserId { get; }
        List<int> RoleIds { get; }

        void SetCurrentUserId(string userId);
        void SetUserRoleIds(string roleIds);
    }
}