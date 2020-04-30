using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.Configuration;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Api.Test
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
        public void Authenticate_Calls_UserService_GetForAuthentication()
        {
            var username = "username";
            var password = "password";
            userAuthenticationService.Authenticate(username, password);
            mockUserService.Verify(s =>
                s.GetForAuthentication(It.IsAny<string>()),
                Times.Once);
        }

        [Test]
        public void Authenticate_Calls_UserPasswordService_ValidateHash()
        {
            var username = "username";
            var password = "password";

            mockUserService.Setup(s => s.GetForAuthentication(It.IsAny<string>())).Returns(new User { IsActive = true });

            userAuthenticationService.Authenticate(username, password);
            mockUserPasswordService.Verify(s =>
                s.ValidateHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                Times.Once);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Authenticate_Returns_String_IfValidated_Null_IfInvalid(bool isValid)
        {
            var username = "username";
            var password = "password";

            mockUserService.Setup(s => s.GetForAuthentication(It.IsAny<string>())).Returns(new User { IsActive = true });
            mockUserPasswordService.Setup(s => s.ValidateHash(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(isValid);

            var result = userAuthenticationService.Authenticate(username, password);
            var wasUserAuthenticated = result != null;

            Assert.AreEqual(wasUserAuthenticated, isValid);
        }
    }
}
