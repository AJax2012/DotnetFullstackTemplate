using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using SourceName.Utils;
using SourceName.Utils.Constants;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task ValidateUser_Calls_UserPasswordValidationService_Validate()
        {
            var password = "test";
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            await userValidationService.ValidateUserAsync(new User { Password = password });
            mockUserPasswordValidationService.Verify(s => s.Validate(password), Times.Once);
        }

        [Test]
        public async Task ValidateUser_Calls_UserService_GetByEmailAddress()
        {
            var password = "test";
            var username = "testUsername";
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            await userValidationService.ValidateUserAsync(new User { Password = password, Username = username });
            mockUserService.Verify(s => s.GetByUsernameAsync(username), Times.Once);
        }

        [Test]
        public async Task ValidateUser_Returns_Valid_UserValidationResult_If_No_Errors()
        {
            var password = "test";
            var username = "testUsername";

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            var actual = await userValidationService.ValidateUserAsync(new User { Password = password, Username = username });

            Assert.IsFalse(actual.Errors.Any());
            Assert.IsTrue(actual.IsValid);
        }

        [Test]
        public async Task ValidateUser_Returns_InValid_UserValidationResult_If_Password_Invalid()
        {
            var password = "test";
            var username = "testusername";
            var errorMessage = "testmessage";
            var expectedResult = new UserValidationResult();
            expectedResult.Errors.Add(errorMessage);
            var passwordErrors = new PasswordValidationResult();
            passwordErrors.Errors.Add(errorMessage);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(passwordErrors);

            var actual = await userValidationService.ValidateUserAsync(new User { Password = password, Username = username });

            Assert.IsTrue(actual.Errors.Any());
            Assert.IsFalse(actual.IsValid);
            Assert.IsTrue(actual.Errors.Contains(errorMessage));
        }

        [Test]
        public async Task ValidateUser_Returns_InValid_UserValidationResult_If_User_Exists()
        {
            var password = "test";
            var username = "testUsername";
            var errorMessage = ErrorStringProvider.UserErrorToString(UserError.EmailExists);
            var expectedResult = new UserValidationResult();
            expectedResult.Errors.Add(errorMessage);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var actual = await userValidationService.ValidateUserAsync(new User { Password = password, Username = username });

            Assert.IsTrue(actual.Errors.Any());
            Assert.IsFalse(actual.IsValid);
            Assert.IsTrue(actual.Errors.Contains(errorMessage));
        }
    }
}
