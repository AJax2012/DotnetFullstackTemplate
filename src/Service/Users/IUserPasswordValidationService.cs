using SourceName.Service.Model.Users;

namespace SourceName.Service.Users
{
    public interface IUserPasswordValidationService
    {
        PasswordValidationResult Validate(string password);
    }
}