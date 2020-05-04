using System;
using SourceName.Service.Model.Users;

namespace SourceName.Service.Users
{
    public interface IUserCapabilitiesService
    {
        UserCapabilities GetUserCapabilities(int userId);
    }
}