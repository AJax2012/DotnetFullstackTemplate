using System.Threading.Tasks;
using SourceName.Service.Model.Users;

namespace SourceName.Service.Users
{
    public interface IUserCapabilitiesService
    {
        Task<UserCapabilities> GetUserCapabilitiesAsync(int userId);
    }
}