using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SourceName.Api.Controllers;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.User;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SourceName.Api.Test
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController usersController;
        private Mock<IMapper> mockMapper;
        private Mock<IUserAuthenticationService> mockUserAuthenticationService;
        private Mock<IUserCapabilitiesService> mockUserCapabilitiesService;
        private Mock<IUserContextService> mockUserContextService;
        private Mock<IUserService> mockUserService;
        private Mock<ILogger<UsersController>> mockLogger;

        [SetUp]
        public void SetUp()
        {
            mockMapper = new Mock<IMapper>();
            mockUserAuthenticationService = new Mock<IUserAuthenticationService>();
            mockUserCapabilitiesService = new Mock<IUserCapabilitiesService>();
            mockUserContextService = new Mock<IUserContextService>();
            mockUserService = new Mock<IUserService>();
            mockLogger = new Mock<ILogger<UsersController>>();
            usersController = new UsersController(mockMapper.Object, mockUserAuthenticationService.Object,
                                                  mockUserCapabilitiesService.Object, mockUserContextService.Object,
                                                  mockUserService.Object, mockLogger.Object);
        }

        [Test]
        public void Authenticate_Calls_UserAuthenticationService_Authenticate()
        {
            usersController.Authenticate(new AuthenticateUserRequest());
            mockUserAuthenticationService.Verify(s => 
                s.Authenticate(It.IsAny<string>(), It.IsAny<string>()), 
                Times.Once);
        }

        [Test]
        public void Authenticate_Calls_UserService_GetUserById()
        {
            mockUserAuthenticationService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "token");

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());

            usersController.Authenticate(new AuthenticateUserRequest());

            mockUserService.Verify(s =>
                s.GetByUsername(It.IsAny<string>()),
                Times.Once);
        }

        [Test]
        public void Authenticate_Returns_OK_Result()
        {
            mockUserAuthenticationService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "token");

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());

            var result = usersController.Authenticate(new AuthenticateUserRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void Authenticate_Returns_NotEmpty_AuthenticateUserResponse()
        {
            var token = "token";

            var user = new User 
            {
                FirstName = "Test",
                LastName = "User",
            };

            var userResponse = new AuthenticateUserResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token
            };

            mockUserAuthenticationService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(() => token);

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(user);

            var result = usersController.Authenticate(new AuthenticateUserRequest()) as OkObjectResult;
            var actual = result.Value as AuthenticateUserResponse;

            Assert.AreEqual(actual.FirstName, userResponse.FirstName);
            Assert.AreEqual(actual.LastName, userResponse.LastName);
            Assert.AreEqual(actual.Token, userResponse.Token);
        }
    }
}
