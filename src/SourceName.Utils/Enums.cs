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
        EmailExists
    }

    public enum PasswordValidationError
    {
        EmptyWhiteSpace,
        PasswordLength,
        LetterAndNumber,
        UpperAndLower,
        Expected64ByteHash,
        Expected128ByteSalt
    }
}
