using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Service.Implementation.Test
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
        [TestCase("Test1")]
        [TestCase("Admin1!")] // default admin password
        public void CreateHash_Returns_Hash_And_Salt_When_Password_Valid(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;
            
            userPasswordService.CreateHash(password, out passwordHash, out passwordSalt);

            var actualHashHasValue = passwordHash != null && passwordHash.Length > 0;
            var actualSaltHasValue = passwordSalt != null && passwordSalt.Length > 0;

            Assert.AreEqual(actualHashHasValue, true);
            Assert.AreEqual(actualSaltHasValue, true);
        }

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("Test")]
        [TestCase("test1")]
        public void CreateHash_ThrowsException_When_Not_Valid(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            Assert.Throws<ArgumentException>(() => userPasswordService.CreateHash(password, out passwordHash, out passwordSalt));
        }

        [Test]
        [TestCase("", "Value cannot be empty or whitespace (Parameter 'password')")]
        [TestCase("  ", "Value cannot be empty or whitespace (Parameter 'password')")]
        [TestCase("Test", "Password must contain at least one letter and one number (Parameter 'password')")]
        [TestCase("test1", "Password must contain at least one upper case and lower case letter (Parameter 'password')")]
        public void CreateHash_Returns_ArgumentExceptionMessage_When_Not_Valid(string password, string expectedExceptionMessage)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            var exception = Assert.Throws<ArgumentException>(() => userPasswordService.CreateHash(password, out passwordHash, out passwordSalt));

            Assert.AreEqual(exception.Message, expectedExceptionMessage);
        }
    }
}
