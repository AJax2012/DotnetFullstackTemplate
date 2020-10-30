using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Users;
using SourceName.Utils;
using SourceName.Utils.Constants;
using SourceName.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Service.Implementation.Test.Users
{
    [TestFixture]
    public class UserPasswordServiceTest
    {
        private UserPasswordService userPasswordService;

        [SetUp]
        public void SetUp()
        {
            userPasswordService = new UserPasswordService();
        }

        [Test]
        public void CreateHash_Returns_Hash_And_Salt_When_Password_Valid()
        {
            var password = "Admin1!";
            byte[] passwordHash;
            byte[] passwordSalt;
            
            userPasswordService.CreateHash(password, out passwordHash, out passwordSalt);

            var actualHashHasValue = passwordHash?.Length > 0;
            var actualSaltHasValue = passwordSalt?.Length > 0;

            Assert.IsTrue(actualHashHasValue);
            Assert.IsTrue(actualSaltHasValue);
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateHash_ThrowsException_When_Password_IsNullOrWhiteSpace(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            Assert.Throws<ArgumentException>(() => userPasswordService.CreateHash(password, out passwordHash, out passwordSalt));
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        public void CreateHash_Returns_ArgumentExceptionMessage_When_Not_Valid(string password)
        {
            var expectedExceptionMessage = new ArgumentException(
                    ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.EmptyWhiteSpace),
                    nameof(password));
            byte[] passwordHash;
            byte[] passwordSalt;

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.CreateHash(password, out passwordHash, out passwordSalt));

            Assert.AreEqual(expectedExceptionMessage.Message, exception.Message);
        }

        [Test]
        public void ValidateHash_Returns_ArgumentException_When_Hash_Not_64_Bytes()
        {
            var password = "Test";
            var passwordHash = new byte[1];
            var passwordSalt = new byte[128];
            var expectedExceptionMessage = new ArgumentException(
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.Expected64ByteHash), 
                nameof(passwordHash));

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.ValidateHash(password, passwordHash, passwordSalt)); ;
            Assert.AreEqual(expectedExceptionMessage.Message, exception.Message);
        }

        [Test]
        public void ValidateHash_Returns_ArgumentException_When_Salt_Not_128_Bytes()
        {
            var password = "Test";
            var passwordHash = new byte[64];
            var passwordSalt = new byte[1];
            var expectedExceptionMessage = new ArgumentException(
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.Expected128ByteSalt), 
                nameof(passwordSalt));

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.ValidateHash(password, passwordHash, passwordSalt));
            Assert.AreEqual(expectedExceptionMessage.Message, exception.Message);
        }
    }
}
