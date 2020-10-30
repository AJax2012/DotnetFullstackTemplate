using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Utils;
using SourceName.Utils.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceName.Service.Implementation.Test.Users
{
    [TestFixture]
    public class UserPasswordValidationServiceTest
    {
        private UserPasswordValidationService userPasswordValidationService;

        [SetUp]
        public void SetUp()
        {
            userPasswordValidationService = new UserPasswordValidationService();
        }

        [Test]
        public void Validate_Returns_Valid_PasswordValidationResult_When_Password_Is_Valid()
        {
            var password = "Test1";
            var result = userPasswordValidationService.Validate(password);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 0);
        }

        [Test]
        public void Validate_Returns_Error_Messages_When_Password_Is_Blank()
        {
            var password = "";
            var expectedErrors = new List<string> 
            { 
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.EmptyWhiteSpace)
            };

            var result = userPasswordValidationService.Validate(password);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsTrue(result.Errors.All(message => expectedErrors.Contains(message)));
        }

        [Test]
        public void Validate_Returns_Error_Messages_When_Password_Is_Spaces()
        {
            var password = "   ";
            var expectedErrors = new List<string>
            {
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.EmptyWhiteSpace)
            };

            var result = userPasswordValidationService.Validate(password);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsTrue(result.Errors.SequenceEqual(expectedErrors));
        }

        [Test]
        public void Validate_Returns_Error_Messages_When_Password_Is_LowerCase()
        {
            var password = "test1";
            var expectedErrors = new List<string>
            {
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.UpperAndLower)
            };

            var result = userPasswordValidationService.Validate(password);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsTrue(result.Errors.All(message => expectedErrors.Contains(message)));
        }

        [Test]
        public void Validate_Returns_Error_Messages_When_Password_Does_Not_Contain_Number()
        {
            var password = "Test";
            var expectedErrors = new List<string>
            {
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.LetterAndNumber)
            };

            var result = userPasswordValidationService.Validate(password);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsTrue(result.Errors.All(message => expectedErrors.Contains(message)));
        }

        [Test]
        public void Validate_Returns_Error_Messages_When_Password_Has_Multiple_Validation_Errors()
        {
            var password = "test";
            var expectedErrors = new List<string>
            {
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.UpperAndLower),
                ErrorStringProvider.PasswordValidationErrorToString(PasswordValidationError.LetterAndNumber)
            };

            var result = userPasswordValidationService.Validate(password);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any());
            Assert.IsTrue(result.Errors.All(message => expectedErrors.Contains(message)));
        }
    }
}
