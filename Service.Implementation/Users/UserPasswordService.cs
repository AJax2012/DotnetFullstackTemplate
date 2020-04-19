using System;
using System.Text.RegularExpressions;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserPasswordService : IUserPasswordService
    {
        public void CreateHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            ValidatePassword(password);
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool ValidateHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace", nameof(password));
            }

            if (passwordHash.Length != 64)
            {
                throw new ArgumentException("Expected 64-byte password hash", nameof(passwordHash));
            }

            if (passwordSalt.Length != 128)
            {
                throw new ArgumentException("Expected 128-byte password salt", nameof(passwordSalt));
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (var x = 0; x < computedHash.Length; x++)
                {
                    if (computedHash[x] != passwordHash[x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace", nameof(password));
            }

            var atLeastOneLetterAndNumber = new Regex(@"[A-Za-z]+\d+.*");
            if (!atLeastOneLetterAndNumber.IsMatch(password))
            {
                throw new ArgumentException("Password must contain at least one letter and one number", nameof(password));
            }

            var atLeastOneUpperAndLower = new Regex(@"(?=.*[a-z])(?=.*[A-Z]).*");
            if (!atLeastOneUpperAndLower.IsMatch(password))
            {
                throw new ArgumentException("Password must contain at least one upper case and lower case letter", nameof(password));
            }

            var repeatingCharacters = new Regex(@"(?!>\w)(\w+?)\1+(?!<\w)");
            if (repeatingCharacters.IsMatch(password))
            {
                throw new ArgumentException("Password may not have repeating characters", nameof(password));
            }
        }
    }
}