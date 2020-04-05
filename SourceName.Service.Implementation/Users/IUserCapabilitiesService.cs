using SourceName.Service.Model.Users;

namespace SourceName.Service.Implementation.Users
{
    public interface IUserCapabilitiesService
    {
        UserCapabilities GetUserCapabilities(int userId);
    }
}