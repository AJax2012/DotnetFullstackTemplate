using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using SourceName.Utils;
using SourceName.Utils.Constants;
using System.Threading.Tasks;

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

        public async Task<UserValidationResult> ValidateUserAsync(User user)
        {
            var result = new UserValidationResult();
            result.Errors.AddRange(_userPasswordValidationService.Validate(user.Password).Errors);

            if (await _userService.GetByUsernameAsync(user.Username) != null)
            {
                result.Errors.Add(ErrorStringProvider.UserErrorToString(UserError.EmailExists));
            }

            return result;
        }
    }
}
