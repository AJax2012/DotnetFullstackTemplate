using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SourceName.Utils
{
    public enum ResponseType
    {
        Success = 0,
        UnauthorizedException = 401,
        Forbidden = 403,
        NotFoundError = 404,
        TimeoutError = 408,
        UnsupportedMediaType = 415,
        ServerError = 500,
        DatabaseError = 600,
    }

    public enum UserError
    {
        [Description("Email Already Exists")]
        EmailExists
    }

    public enum PasswordValidationError
    {
        [Description("Value cannot be empty or whitespace")]
        EmptyWhiteSpace,

        [Description("Password must be between 6 and 20 characters in length")]
        PasswordLength,

        [Description("Password must contain at least one letter and one number")]
        LetterAndNumber,

        [Description("Password must contain at least one upper case and lower case letter")]
        UpperAndLower,

        [Description("Expected 64-byte password hash")]
        Expected64ByteHash,

        [Description("Expected 128-byte password salt")]
        Expected128ByteSalt
    }
}
