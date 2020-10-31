using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using SourceName.Utils;
using SourceName.Utils.Constants;
using SourceName.Utils.Constants.EnumDescriptionProviders;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SourceName.Service.Implementation.Users
{
    public class UserPasswordValidationService : IUserPasswordValidationService
    {
        private Dictionary<Regex, string> ValidationRules = new Dictionary<Regex, string>
        {
            { new Regex(@"[A-Za-z]+\d+.*"), 
                PasswordValidationError.LetterAndNumber.ToDescriptionString() },

            { new Regex(@"(?=.*[a-z])(?=.*[A-Z]).*"), 
                PasswordValidationError.UpperAndLower.ToDescriptionString() }
        };

        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add(PasswordValidationError.EmptyWhiteSpace.ToDescriptionString());
                return result;
            }

            foreach (var rule in ValidationRules)
            {
                if (!rule.Key.IsMatch(password))
                {
                    result.Errors.Add(rule.Value);
                }
            }

            return result;
        }
    }
}
