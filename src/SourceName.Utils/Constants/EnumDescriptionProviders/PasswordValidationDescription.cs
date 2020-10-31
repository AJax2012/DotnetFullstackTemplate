using SourceName.Utils.Interfaces;
using System;

namespace SourceName.Utils.Constants.EnumDescriptionProviders
{
    public static class PasswordValidationDescription
    {
        public static string ToDescriptionString(this PasswordValidationError error)
        {
            return error switch
            {
                PasswordValidationError.EmptyWhiteSpace => "Value cannot be empty or whitespace",
                PasswordValidationError.PasswordLength => "Password must be between 6 and 20 characters in length",
                PasswordValidationError.LetterAndNumber => "Password must contain at least one letter and one number",
                PasswordValidationError.UpperAndLower => "Password must contain at least one upper case and lower case letter",
                PasswordValidationError.Expected64ByteHash => "Expected 64-byte password hash",
                PasswordValidationError.Expected128ByteSalt => "Expected 128-byte password salt",
                _ => throw new NotImplementedException(),
            };
        }
    }
}
