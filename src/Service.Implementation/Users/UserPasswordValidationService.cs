using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SourceName.Service.Implementation.Users
{
    public class UserPasswordValidationService : IUserPasswordValidationService
    {
        private Dictionary<Regex, string> ValidationRules = new Dictionary<Regex, string>
        {
            { new Regex(@"[A-Za-z]+\d+.*"), "Password must contain at least one letter and one number" },
            { new Regex(@"(?=.*[a-z])(?=.*[A-Z]).*"), "Password must contain at least one upper case and lower case letter" }
        };

        public PasswordValidationResult Validate(string password)
        {
            var result = new PasswordValidationResult();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add("Value cannot be empty or whitespace");
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
