using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserValidationService : IUserValidationService
    {
        private readonly IUserService _userService;
        private readonly IUserPasswordValidationService _userPasswordValidationService;

        public UserValidationService(IUserService userService, IUserPasswordValidationService userPasswordValidationService)
        {
            _userService = userService;
            _userPasswordValidationService = userPasswordValidationService;
        }

        public UserValidationResult ValidateUser(User user)
        {
            var result = new UserValidationResult();
            result.Errors.AddRange(_userPasswordValidationService.Validate(user.Password).Errors);

            if (_userService.GetByUsername(user.Username) != null)
            {
                result.Errors.Add("Email Already Exists");
            }

            return result;
        }
    }
}
