namespace SourceName.Api.Core.Authentication
{
    public interface IUserAuthenticationService
    {
        string Authenticate(string username, string password);
    }
}