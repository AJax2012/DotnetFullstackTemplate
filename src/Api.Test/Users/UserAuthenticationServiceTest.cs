using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SourceName.Api.Core.Authentication;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using SourceName.Utils.Configuration;
using System.Threading.Tasks;

namespace SourceName.Api.Test.Users
{
    [TestFixture]
    public class UserAuthenticationServiceTest
    {
        private Mock<IOptionsSnapshot<SecretsConfiguration>> mockSecretsConfiguration;
        private Mock<IUserPasswordService> mockUserPasswordService;
        private Mock<IUserService> mockUserService;
        private UserAuthenticationService userAuthenticationService;

        [SetUp]
        public void SetUp()
        {
            var secretsConfigValues = new SecretsConfiguration 
            {
                UserPasswordSecret = "ThisIsTopSecretHashYouKnowNothing"
            };

            mockSecretsConfiguration = new Mock<IOptionsSnapshot<SecretsConfiguration>>();
            mockSecretsConfiguration.Setup(secret => secret.Value).Returns(secretsConfigValues);
            mockUserPasswordService = new Mock<IUserPasswordService>();
            mockUserService = new Mock<IUserService>();
            userAuthenticationService = new UserAuthenticationService(mockSecretsConfiguration.Object, mockUserPasswordService.Object, mockUserService.Object);
        }

        [Test]
        public async Task Authenticate_Calls_UserService_GetForAuthentication()
        {
            var username = "username";
            var password = "password";
            await userAuthenticationService.AuthenticateAsync(username, password);
            mockUserService.Verify(s =>
                s.GetForAuthenticationAsync(It.IsAny<string>()),
                Times.Once);
        }

        [Test]
        public async Task Authenticate_Calls_UserPasswordService_ValidateHash()
        {
            var username = "username";
            var password = "password";

            mockUserService.Setup(s => s.GetForAuthenticationAsync(It.IsAny<string>())).ReturnsAsync(new User { IsActive = true });

            await userAuthenticationService.AuthenticateAsync(username, password);
            mockUserPasswordService.Verify(s =>
                s.ValidateHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                Times.Once);
        }

        [Test]
        public async Task Authenticate_Returns_Null_Value_If_User_Not_Found()
        {
            var username = "username";
            var password = "password";

            var result = await userAuthenticationService.AuthenticateAsync(username, password);

            Assert.IsNull(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Authenticate_Returns_NotNull_Value_If_User_IsActive_Null_If_User_Not_IsActive(bool isActive)
        {
            var username = "username";
            var password = "password";

            mockUserService.Setup(s => s.GetForAuthenticationAsync(It.IsAny<string>())).ReturnsAsync(new User { IsActive = isActive });
            mockUserPasswordService.Setup(s => s.ValidateHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);

            var result = await userAuthenticationService.AuthenticateAsync(username, password);
            var wasUserAuthenticated = result != null;

            Assert.AreEqual(isActive, wasUserAuthenticated);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Authenticate_Returns_NotNull_Value_IfValidated_Null_IfInvalid(bool isValid)
        {
            var username = "username";
            var password = "password";

            mockUserService.Setup(s => s.GetForAuthenticationAsync(It.IsAny<string>())).ReturnsAsync(new User { IsActive = true });
            mockUserPasswordService.Setup(s => s.ValidateHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(isValid);

            var result = await userAuthenticationService.AuthenticateAsync(username, password);
            var wasUserAuthenticated = result != null;

            Assert.AreEqual(isValid, wasUserAuthenticated);
        }
    }
}
