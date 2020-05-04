using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System.Text.RegularExpressions;

namespace SourceName.Service.Implementation.Users
{
    public class UserPasswordValidationService : IUserPasswordValidationService
    {
        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add("Value cannot be empty or whitespace");
                return result;
            }

            var atLeastOneLetterAndNumber = new Regex(@"[A-Za-z]+\d+.*");
            if (!atLeastOneLetterAndNumber.IsMatch(password))
            {
                result.Errors.Add("Password must contain at least one letter and one number");
            }

            var atLeastOneUpperAndLower = new Regex(@"(?=.*[a-z])(?=.*[A-Z]).*");
            if (!atLeastOneUpperAndLower.IsMatch(password))
            {
                result.Errors.Add("Password must contain at least one upper case and lower case letter");
            }

            var repeatingCharacters = new Regex(@"(?!>\w)(\w+?)\1+(?!<\w)");
            if (repeatingCharacters.IsMatch(password))
            {
                result.Errors.Add("Password may not have repeating characters");
            }

            return result;
        }
    }
}
