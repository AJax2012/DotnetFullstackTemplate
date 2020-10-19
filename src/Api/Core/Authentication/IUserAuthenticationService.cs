using System.Threading.Tasks;

namespace SourceName.Api.Core.Authentication
{
    public interface IUserAuthenticationService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}