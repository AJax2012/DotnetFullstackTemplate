using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceName.Service.Test.Users
{
    [TestFixture]
    public class UserValidationResultTest
    {
        private Mock<IUserService> mockUserService;
        private Mock<IUserPasswordValidationService> mockUserPasswordValidationService;
        private IUserValidationService userValidationService;

        [SetUp]
        public void SetUp()
        {
            mockUserService = new Mock<IUserService>();
            mockUserPasswordValidationService = new Mock<IUserPasswordValidationService>();
            userValidationService = new UserValidationService(mockUserService.Object, mockUserPasswordValidationService.Object);
        }

        [Test]
        public void ValidateUser_Calls_UserPasswordValidationService_Validate()
        {
            var password = "test";
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            userValidationService.ValidateUser(new User { Password = password });
            mockUserPasswordValidationService.Verify(s => s.Validate(password), Times.Once);
        }

        [Test]
        public void ValidateUser_Calls_UserService_GetByEmailAddress()
        {
            var password = "test";
            var username = "testUsername";
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            userValidationService.ValidateUser(new User { Password = password, Username = username });
            mockUserService.Verify(s => s.GetByUsername(username), Times.Once);
        }

        [Test]
        public void ValidateUser_Returns_Valid_UserValidationResult_If_No_Errors()
        {
            var password = "test";
            var username = "testUsername";

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            var actual = userValidationService.ValidateUser(new User { Password = password, Username = username });

            Assert.IsFalse(actual.Errors.Any());
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public void ValidateUser_Returns_InValid_UserValidationResult_If_Password_Invalid()
        {
            var password = "test";
            var username = "testusername";
            var errorMessage = "testmessage";
            var expectedResult = new UserValidationResult();
            expectedResult.Errors.Add(errorMessage);
            var passwordErrors = new PasswordValidationResult();
            passwordErrors.Errors.Add(errorMessage);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(passwordErrors);

            var actual = userValidationService.ValidateUser(new User { Password = password, Username = username });

            Assert.IsTrue(actual.Errors.Any());
            Assert.IsFalse(actual.IsValid);
            Assert.IsTrue(actual.Errors.Contains(errorMessage));
        }

        [Test]
        public void ValidateUser_Returns_InValid_UserValidationResult_If_User_Exists()
        {
            var password = "test";
            var username = "testUsername";
            var errorMessage = "Email Already Exists";
            var expectedResult = new UserValidationResult();
            expectedResult.Errors.Add(errorMessage);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());

            var actual = userValidationService.ValidateUser(new User { Password = password, Username = username });

            Assert.IsTrue(actual.Errors.Any());
            Assert.IsFalse(actual.IsValid);
            Assert.IsTrue(actual.Errors.Contains(errorMessage));
        }
    }
}
