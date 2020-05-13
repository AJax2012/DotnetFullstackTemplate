using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Users;
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

            Assert.AreEqual(actualHashHasValue, true);
            Assert.AreEqual(actualSaltHasValue, true);
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
            var expectedExceptionMessage = "Value cannot be empty or whitespace (Parameter 'password')";
            byte[] passwordHash;
            byte[] passwordSalt;

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.CreateHash(password, out passwordHash, out passwordSalt));

            Assert.AreEqual(exception.Message, expectedExceptionMessage);
        }

        [Test]
        public void ValidateHash_Returns_ArgumentException_When_Hash_Not_64_Bytes()
        {
            var password = "Test";
            var passwordHash = new byte[1];
            var passwordSalt = new byte[128];
            var expectedExceptionMessage = "Expected 64-byte password hash (Parameter 'passwordHash')";

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.ValidateHash(password, passwordHash, passwordSalt)); ;
            Assert.AreEqual(exception.Message, expectedExceptionMessage);
        }

        [Test]
        public void ValidateHash_Returns_ArgumentException_When_Salt_Not_128_Bytes()
        {
            var password = "Test";
            var passwordHash = new byte[64];
            var passwordSalt = new byte[1];
            var expectedExceptionMessage = "Expected 128-byte password salt (Parameter 'passwordSalt')";

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.ValidateHash(password, passwordHash, passwordSalt));
            Assert.AreEqual(exception.Message, expectedExceptionMessage);
        }
    }
}
