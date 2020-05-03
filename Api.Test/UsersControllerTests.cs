﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SourceName.Api.Controllers;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.User;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System.Linq;
using System.Collections.Generic;
using System;

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
            var request = new AuthenticateUserRequest { Username = "test", Password = "Admin1!" }; // password is the same as the default admin password
            usersController.Authenticate(request);
            mockUserAuthenticationService.Verify(s => 
                s.Authenticate(
                    It.Is<string>(username => username == request.Username), 
                    It.Is<string>(password => password == request.Password)),
                Times.Once);
        }

        [Test]
        public void Authenticate_Logs_When_Token_Null()
        {
            usersController.Authenticate(new AuthenticateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void Authenticate_Calls_UserService_GetUserByUsername()
        {
            var request = new AuthenticateUserRequest { Username = "test", Password = "Admin1!" }; // password is the same as the default admin password

            mockUserAuthenticationService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "token");
            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());

            usersController.Authenticate(request);

            mockUserService.Verify(s =>
                s.GetByUsername(It.Is<string>(username => username == request.Username)),
                Times.Once);
        }

        [Test]
        public void Authenticate_Returns_Ok_Object_Result()
        {
            mockUserAuthenticationService.Setup(s => s.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(() => "token");
            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());

            var result = usersController.Authenticate(new AuthenticateUserRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void Authenticate_Returns_Predicted_AuthenticateUserResponse()
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

        [Test]
        public void Authenticate_Returns_Unauthorized_If_Invalid_Login()
        {
            var result = usersController.Authenticate(new AuthenticateUserRequest()) as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public void Register_Calls_UserService_GetByUsername()
        {
            var username = "test";

            usersController.Register(new CreateUserRequest { Username = username });

            mockUserService.Verify(s => s.GetByUsername(username), Times.Once);
        }

        [Test]
        public void Register_Calls_Automapper_Map_To_User()
        {
            var request = new CreateUserRequest();

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(new User());
            usersController.Register(request);

            mockMapper.Verify(m => m.Map<User>(It.Is<CreateUserRequest>(r => r == request)));
        }

        [Test]
        public void Register_Calls_UserService_CreateUser()
        {
            var user = new User();

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>())).Returns(user);
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(new User());

            usersController.Register(new CreateUserRequest());

            mockUserService.Verify(s => s.CreateUser(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public void Register_Logs()
        {
            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(new User());

            usersController.Register(new CreateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void Register_Calls_Automapper_Map_To_UserResource()
        {
            var request = new User();

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(request);
            
            usersController.Register(new CreateUserRequest());

            mockMapper.Verify(m => m.Map<UserResource>(It.Is<User>(r => r == request)));
        }

        [Test]
        public void Register_Returns_Created_Result()
        {
            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(new User());

            var result = usersController.Register(new CreateUserRequest()) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }

        [Test]
        public void Register_Returns_CreateUserResource_With_UserResource_If_User_Not_Existed()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetByUsername(It.IsAny<string>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUser(It.IsAny<User>())).Returns(new User());

            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = usersController.Register(new CreateUserRequest()) as CreatedAtActionResult;
            var actual = result.Value as CreateUserResponse;

            Assert.AreEqual(actual.UserResource, resource);
            Assert.AreEqual(actual.IsUserCreated, true);
        }

        [Test]
        public void Register_Returns_CreateUserResource_Without_UserResource_If_User_Existed()
        {
            var request = new CreateUserRequest { Username = "test" };
            var result = usersController.Register(request) as OkObjectResult;
            var actual = result.Value as CreateUserResponse;

            Assert.AreEqual(actual.IsUserCreated, false);
            Assert.IsNull(actual.UserResource);
            Assert.IsNotEmpty(actual.Message);
        }

        [Test]
        public void DeleteUser_Calls_UserService_GetById()
        {
            var userId = 1;
            usersController.DeleteUser(userId);
            mockUserService.Verify(s => s.GetById(userId), Times.Once);
        }

        [Test]
        public void DeleteUser_Logs_When_User_Found()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());

            usersController.DeleteUser(new int());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void DeleteUser_Calls_UserService_DeleteUser()
        {
            var userId = 1;
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            usersController.DeleteUser(userId);
            mockUserService.Verify(s => s.DeleteUser(userId), Times.Once);
        }

        [Test]
        public void DeleteUser_Returns_NoContent_Result_If_User_Exists()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            var result = usersController.DeleteUser(new int()) as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public void DeleteUser_Returns_NotFound_Result_If_User_Not_Exists()
        {
            var result = usersController.DeleteUser(new int()) as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetAll_Calls_UserService_GetAll()
        {
            usersController.GetAll();
            mockUserService.Verify(s => s.GetAll(), Times.Once);
        }

        [Test]
        public void GetAll_Maps_To_UserResource()
        {
            var users = new List<User> { new User() };

            mockUserService.Setup(s => s.GetAll()).Returns(users);
            usersController.GetAll();

            mockMapper.Verify(m => m.Map<List<UserResource>>(It.Is<List<User>>(u => u == users)));
        }

        [Test]
        public void GetAll_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetAll()).Returns(new List<User>());

            var result = usersController.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetAll_Returns_NotEmpty_UserResourceList()
        {
            mockUserService.Setup(s => s.GetAll()).Returns(new List<User>());

            mockMapper.Setup(m => m.Map<List<UserResource>>(It.IsAny<List<User>>())).Returns(new List<UserResource> { new UserResource() });

            var result = usersController.GetAll() as OkObjectResult;
            var actual = result.Value as List<UserResource>;

            Assert.IsTrue(actual.Any());
        }

        [Test]
        public void GetAll_Returns_UserResourceList()
        {
            var userId = 1;
            var resource = new List<UserResource> { new UserResource { Id = userId } };

            mockUserService.Setup(s => s.GetAll()).Returns(new List<User>());

            mockMapper.Setup(m => m.Map<List<UserResource>>(It.IsAny<List<User>>())).Returns(resource);

            var result = usersController.GetAll() as OkObjectResult;
            var actual = result.Value as List<UserResource>;

            Assert.AreEqual(actual, resource);
        }

        [Test]
        public void GetById_Calls_UserService_GetById()
        {
            var userId = 1;
            usersController.GetById(userId);
            mockUserService.Verify(s => s.GetById(userId), Times.Once);
        }

        [Test]
        public void GetById_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());

            var result = usersController.GetById(new int()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetById_Returns_UserResource_If_User_Found()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());

            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = usersController.GetById(new int()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(actual, resource);
        }

        [Test]
        public void GetById_Returns_NotFoundResult_If_User_Not_Found()
        {
            var result = usersController.GetById(new int()) as NotFoundResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetUserCapabilities_Calls_GetUserCapabilities()
        {
            var userId = 1;
            mockUserContextService.Setup(s => s.UserId).Returns(userId);
            usersController.GetUserCapabilities();
            mockUserCapabilitiesService.Verify(s => s.GetUserCapabilities(userId), Times.Once);
        }

        [Test]
        public void GetUserCapabilities_Returns_Ok_Object_Result()
        {
            mockUserContextService.Setup(s => s.UserId).Returns(new int());
            mockUserCapabilitiesService.Setup(s => s.GetUserCapabilities(It.IsAny<int>())).Returns(new UserCapabilities());

            var result = usersController.GetUserCapabilities() as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetUserCapabilities_Returns_UserCapabilities()
        {
            var resource = new UserCapabilitiesResource { CanManageUsers = true };

            mockUserContextService.Setup(s => s.UserId).Returns(new int());
            mockUserCapabilitiesService.Setup(s => s.GetUserCapabilities(It.IsAny<int>())).Returns(new UserCapabilities());
            mockMapper.Setup(m => m.Map<UserCapabilitiesResource>(It.IsAny<UserCapabilities>())).Returns(resource);

            var result = usersController.GetUserCapabilities() as OkObjectResult;
            var actual = result.Value as UserCapabilitiesResource;

            Assert.AreEqual(actual, resource);
        }

        [Test]
        public void UpdateUser_Calls_UserService_GetById()
        {
            var userId = 1;
            usersController.UpdateUser(userId, new UpdateUserRequest());
            mockUserService.Verify(s => s.GetById(userId), Times.Once);
        }

        [Test]
        public void UpdateUser_Logs_When_User_Found()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            usersController.UpdateUser(new int(), new UpdateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void UpdateUser_Maps_UpdateUserRequest_To_User()
        {
            var request = new UpdateUserRequest();

            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            usersController.UpdateUser(new int(), request);

            mockMapper.Verify(m => m.Map<User>(It.Is<UpdateUserRequest>(ur => ur == request)));
        }

        [Test]
        public void UpdateUser_Calls_UserService_UpdateUser()
        {
            var user = new User();
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(user);
            usersController.UpdateUser(new int(), new UpdateUserRequest());
            mockUserService.Verify(s => s.UpdateUser(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public void UpdateUser_Maps_User_To_UserResource()
        {
            var user = new User();
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUser(It.IsAny<User>())).Returns(user);

            usersController.UpdateUser(new int(), new UpdateUserRequest());

            mockMapper.Verify(s => s.Map<UserResource>(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public void UpdateUser_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            mockUserService.Setup(s => s.UpdateUser(It.IsAny<User>())).Returns(new User());

            var result = usersController.UpdateUser(new int(), new UpdateUserRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void UpdateUser_Returns_UserResource_If_User_Found()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUser(It.IsAny<User>())).Returns(new User());
            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = usersController.UpdateUser(new int(), new UpdateUserRequest()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(actual, resource);
        }

        [Test]
        public void UpdateUser_Returns_NotFoundResult_If_User_Not_Found()
        {
            var result = usersController.UpdateUser(new int(), new UpdateUserRequest()) as NotFoundResult;
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void UpdatePassword_Calls_UserService_GetById()
        {
            var userId = 1;
            usersController.UpdatePassword(userId, new UpdatePasswordRequest());
            mockUserService.Verify(s => s.GetById(userId), Times.Once);
        }

        [Test]
        public void UpdatePassword_Logs_When_User_Found()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            usersController.UpdatePassword(new int(), new UpdatePasswordRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void UpdatePassword_Calls_UserService_UpdateUser()
        {
            var userId = 1;
            var userPassword = "Admin1!"; // password used for default Admin account
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            usersController.UpdatePassword(userId, new UpdatePasswordRequest { Password = userPassword });
            mockUserService.Verify(s => s.UpdateUserPassword(userId, userPassword), Times.Once);
        }

        [Test]
        public void UpdatePassword_Maps_User_To_UserResource()
        {
            var resource = new User();
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUserPassword(It.IsAny<int>(), It.IsAny<string>())).Returns(resource);

            usersController.UpdatePassword(new int(), new UpdatePasswordRequest());

            mockMapper.Verify(m => m.Map<UserResource>(It.Is<User>(r => r == resource)));
        }

        [Test]
        public void UpdatePassword_Returns_OkObjectResult_If_User_Found()
        {
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUserPassword(It.IsAny<int>(), It.IsAny<string>())).Returns(new User());

            var result = usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void UpdatePassword_Returns_NotFoundResult_If_User_Not_Found()
        {
            var result = usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as NotFoundResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void UpdatePassword_Returns_UserResource()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUserPassword(It.IsAny<int>(), It.IsAny<string>())).Returns(new User());
            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(actual, resource);
        }
    }
}
