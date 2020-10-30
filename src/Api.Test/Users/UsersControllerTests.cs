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
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SourceName.Data.Model;
using SourceName.Api.Model;
using SourceName.Service.Model;

namespace SourceName.Api.Test.Users
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController usersController;
        private Mock<IMapper> mockMapper;
        private Mock<IUserPasswordValidationService> mockUserPasswordValidationService;
        private Mock<IUserValidationService> mockUserValidationService;
        private Mock<IUserAuthenticationService> mockUserAuthenticationService;
        private Mock<IUserCapabilitiesService> mockUserCapabilitiesService;
        private Mock<IUserContextService> mockUserContextService;
        private Mock<IUserService> mockUserService;
        private Mock<ILogger<UsersController>> mockLogger;

        [SetUp]
        public void SetUp()
        {
            mockMapper = new Mock<IMapper>();
            mockUserPasswordValidationService = new Mock<IUserPasswordValidationService>();
            mockUserValidationService = new Mock<IUserValidationService>();
            mockUserAuthenticationService = new Mock<IUserAuthenticationService>();
            mockUserCapabilitiesService = new Mock<IUserCapabilitiesService>();
            mockUserContextService = new Mock<IUserContextService>();
            mockUserService = new Mock<IUserService>();
            mockLogger = new Mock<ILogger<UsersController>>();
            usersController = new UsersController(mockMapper.Object, mockUserPasswordValidationService.Object, mockUserValidationService.Object, mockUserAuthenticationService.Object,
                                                  mockUserCapabilitiesService.Object, mockUserContextService.Object,
                                                  mockUserService.Object, mockLogger.Object);
        }

        [Test]
        public async Task Authenticate_Calls_UserAuthenticationService_Authenticate()
        {
            var request = new AuthenticateUserRequest { Username = "test", Password = "Admin1!" }; // password is the same as the default admin password
            await usersController.AuthenticateAsync(request);
            mockUserAuthenticationService.Verify(s => 
                s.AuthenticateAsync(
                    It.Is<string>(username => username == request.Username), 
                    It.Is<string>(password => password == request.Password)),
                Times.Once);
        }

        [Test]
        public async Task Authenticate_Logs_When_Token_Null()
        {
            await usersController.AuthenticateAsync(new AuthenticateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task Authenticate_Calls_UserService_GetUserByUsername()
        {
            var request = new AuthenticateUserRequest { Username = "test", Password = "Admin1!" }; // password is the same as the default admin password

            mockUserAuthenticationService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => "token");
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            await usersController.AuthenticateAsync(request);

            mockUserService.Verify(s =>
                s.GetByUsernameAsync(It.Is<string>(username => username == request.Username)),
                Times.Once);
        }

        [Test]
        public async Task Authenticate_Returns_Ok_Object_Result()
        {
            mockUserAuthenticationService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => "token");
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var result = await usersController.AuthenticateAsync(new AuthenticateUserRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task Authenticate_Returns_Predicted_AuthenticateUserResponse()
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

            mockUserAuthenticationService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => token);

            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(user);

            var result = await usersController.AuthenticateAsync(new AuthenticateUserRequest()) as OkObjectResult;
            var actual = result.Value as AuthenticateUserResponse;

            Assert.AreEqual(userResponse.FirstName, actual.FirstName);
            Assert.AreEqual(userResponse.LastName, actual.LastName);
            Assert.AreEqual(userResponse.Token, actual.Token);
        }

        [Test]
        public async Task Authenticate_Returns_Unauthorized_If_Invalid_Login()
        {
            var result = await usersController.AuthenticateAsync(new AuthenticateUserRequest()) as UnauthorizedResult;
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public async Task Register_Calls_UserValidationService_ValidateUser()
        {
            var user = new User();

            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>())).Returns(user);
            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = new int() });

            await usersController.RegisterAsync(new CreateUserRequest());
            mockUserValidationService.Verify(s => s.ValidateUserAsync(user), Times.Once);
        }

        [Test]
        public async Task Register_Calls_Automapper_Map_To_User()
        {
            var request = new CreateUserRequest();

            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());
            await usersController.RegisterAsync(request);

            mockMapper.Verify(m => m.Map<User>(It.Is<CreateUserRequest>(r => r == request)));
        }

        [Test]
        public async Task Register_Calls_UserService_CreateUser()
        {
            var user = new User();

            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>())).Returns(user);
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());

            await usersController.RegisterAsync(new CreateUserRequest());

            mockUserService.Verify(s => s.CreateUserAsync(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public async Task Register_Logs()
        {
            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<CreateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());

            await usersController.RegisterAsync(new CreateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task Register_Calls_Automapper_Map_To_UserResource()
        {
            var request = new User();

            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(request);
            
            await usersController.RegisterAsync(new CreateUserRequest());

            mockMapper.Verify(m => m.Map<UserResource>(It.Is<User>(r => r == request)));
        }

        [Test]
        public async Task Register_Returns_Created_Result()
        {
            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());

            var result = await usersController.RegisterAsync(new CreateUserRequest()) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.AreEqual(201, result.StatusCode);
        }

        [Test]
        public async Task Register_Returns_BadRequestObjectResult_With_PasswordValidationResult_If_Password_Invalid()
        {
            var error = "test error";
            var validationResult = new UserValidationResult();
            validationResult.Errors.Add(error);

            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(validationResult);
            var request = new CreateUserRequest();
            var result = await usersController.RegisterAsync(request) as OkObjectResult;
            var actual = result.Value as UserValidationResult;

            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(validationResult, actual);
        }

        [Test]
        public async Task Register_Returns_CreateUserResource_With_UserResource_If_User_Not_Existed()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserValidationService.Setup(s => s.ValidateUserAsync(It.IsAny<User>())).ReturnsAsync(new UserValidationResult());
            mockUserService.Setup(s => s.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());

            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = await usersController.RegisterAsync(new CreateUserRequest()) as CreatedAtActionResult;
            var actual = result.Value as CreateUserResponse;

            Assert.AreEqual(resource, actual.UserResource);
            Assert.IsTrue(actual.IsUserCreated);
        }

        [Test]
        public async Task DeleteUser_Calls_UserService_GetById()
        {
            var userId = 1;
            await usersController.DeleteUser(userId);
            mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task DeleteUser_Logs_When_User_Found()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());

            await usersController.DeleteUser(new int());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task DeleteUser_Calls_UserService_DeleteUser()
        {
            var userId = 1;
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            await usersController.DeleteUser(userId);
            mockUserService.Verify(s => s.DeleteUserAsync(userId), Times.Once);
        }

        [Test]
        public async Task DeleteUser_Returns_500_If_ServerError()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<int>())).ReturnsAsync(false);
            var result = await usersController.DeleteUser(new int()) as StatusCodeResult;
            Assert.AreEqual(500, result.StatusCode);
        }

        [Test]
        public async Task DeleteUser_Returns_NoContent_Result_If_User_Exists()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<int>())).ReturnsAsync(true);
            var result = await usersController.DeleteUser(new int()) as NoContentResult;
            Assert.AreEqual(204, result.StatusCode);
        }

        [Test]
        public async Task DeleteUser_Returns_NotFound_Result_If_User_Not_Exists()
        {
            var result = await usersController.DeleteUser(new int()) as NotFoundResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task GetAll_Calls_UserService_GetAll()
        {
            await usersController.GetAll();
            mockUserService.Verify(s => s.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task GetAll_Maps_To_UserResource()
        {
            var users = new SearchResult<User>();

            mockUserService.Setup(s => s.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(users);
            await usersController.GetAll();

            mockMapper.Verify(m => m.Map<SearchResultResource<UserResource>>(It.Is<SearchResult<User>>(u => u == users)));
        }

        [Test]
        public async Task GetAll_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new SearchResult<User>());

            var result = await usersController.GetAll() as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetAll_Returns_NotEmpty_UserResourceList()
        {
            var user = new UserResource();
            var userList = new List<UserResource> { user };
            var expectedResult = new SearchResultResource<UserResource> { Data = userList, TotalCount = userList.Count };

            mockUserService.Setup(s => s.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new SearchResult<User>());

            mockMapper.Setup(m => m.Map<SearchResultResource<UserResource>>(It.IsAny<SearchResult<User>>())).Returns(expectedResult);

            var result = await usersController.GetAll() as OkObjectResult;
            var actual = result.Value as SearchResultResource<UserResource>;

            Assert.IsNotEmpty(actual.Data);
        }

        [Test]
        public async Task GetAll_Returns_UserResourceList()
        {
            var userId = 1;
            var user = new UserResource { Id = userId };
            var userList = new List<UserResource> { user };
            var resource = new SearchResultResource<UserResource> { Data = userList, TotalCount = userList.Count };

            mockUserService.Setup(s => s.GetAllPaginatedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(new SearchResult<User>());

            mockMapper.Setup(m => m.Map<SearchResultResource<UserResource>>(It.IsAny<SearchResult<User>>())).Returns(resource);

            var result = await usersController.GetAll() as OkObjectResult;
            var actual = result.Value as SearchResultResource<UserResource>;

            Assert.AreEqual(resource, actual);
        }

        [Test]
        public async Task GetById_Calls_UserService_GetById()
        {
            var userId = 1;
            await usersController.GetById(userId);
            mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetById_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());

            var result = await usersController.GetById(new int()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetById_Returns_UserResource_If_User_Found()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());

            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = await usersController.GetById(new int()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(resource, actual);
        }

        [Test]
        public async Task GetById_Returns_NotFoundResult_If_User_Not_Found()
        {
            var result = await usersController.GetById(new int()) as NotFoundResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task GetUserCapabilities_Calls_GetUserCapabilities()
        {
            var userId = 1;
            mockUserContextService.Setup(s => s.UserId).Returns(userId);
            await usersController.GetUserCapabilities();
            mockUserCapabilitiesService.Verify(s => s.GetUserCapabilitiesAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserCapabilities_Returns_Ok_Object_Result()
        {
            mockUserContextService.Setup(s => s.UserId).Returns(new int());
            mockUserCapabilitiesService.Setup(s => s.GetUserCapabilitiesAsync(It.IsAny<int>())).ReturnsAsync(new UserCapabilities());

            var result = await usersController.GetUserCapabilities() as OkObjectResult;
            
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task GetUserCapabilities_Returns_UserCapabilities()
        {
            var resource = new UserCapabilitiesResource { CanManageUsers = true };

            mockUserContextService.Setup(s => s.UserId).Returns(new int());
            mockUserCapabilitiesService.Setup(s => s.GetUserCapabilitiesAsync(It.IsAny<int>())).ReturnsAsync(new UserCapabilities());
            mockMapper.Setup(m => m.Map<UserCapabilitiesResource>(It.IsAny<UserCapabilities>())).Returns(resource);

            var result = await usersController.GetUserCapabilities() as OkObjectResult;
            var actual = result.Value as UserCapabilitiesResource;

            Assert.AreEqual(resource, actual);
        }

        [Test]
        public async Task UpdateUser_Calls_UserService_GetById()
        {
            var userId = 1;
            await usersController.UpdateUser(userId, new UpdateUserRequest());
            mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Logs_When_User_Found()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            await usersController.UpdateUser(new int(), new UpdateUserRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Maps_UpdateUserRequest_To_User()
        {
            var request = new UpdateUserRequest();

            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            await usersController.UpdateUser(new int(), request);

            mockMapper.Verify(m => m.Map<User>(It.Is<UpdateUserRequest>(ur => ur == request)));
        }

        [Test]
        public async Task UpdateUser_Calls_UserService_UpdateUser()
        {
            var user = new User();
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(user);
            await usersController.UpdateUser(new int(), new UpdateUserRequest());
            mockUserService.Verify(s => s.UpdateUserAsync(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Maps_User_To_UserResource()
        {
            var user = new User();
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(user);

            await usersController.UpdateUser(new int(), new UpdateUserRequest());

            mockMapper.Verify(s => s.Map<UserResource>(It.Is<User>(u => u == user)), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Returns_Ok_Object_Result()
        {
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());

            var result = await usersController.UpdateUser(new int(), new UpdateUserRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task UpdateUser_Returns_UserResource_If_User_Found()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());
            mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = await usersController.UpdateUser(new int(), new UpdateUserRequest()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(resource, actual);
        }

        [Test]
        public async Task UpdateUser_Returns_NotFoundResult_If_User_Not_Found()
        {
            var result = await usersController.UpdateUser(new int(), new UpdateUserRequest()) as NotFoundResult;
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task UpdatePassword_Calls_UserPasswordValidationService_Validate()
        {
            var password = "test";
            var error = "test error";
            var validationResult = new PasswordValidationResult();
            validationResult.Errors.Add(error);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(validationResult);
            await usersController.UpdatePassword(new int(), new UpdatePasswordRequest { Password = password });
            mockUserPasswordValidationService.Verify(s => s.Validate(password), Times.Once);
        }

        [Test]
        public async Task UpdatePassword_Calls_UserService_GetById()
        {
            var userId = 1;
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());

            await usersController.UpdatePassword(userId, new UpdatePasswordRequest());
            mockUserService.Verify(s => s.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task UpdatePassword_Logs_When_User_Found()
        {
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UpdateUserRequest>())).Returns(new User());

            await usersController.UpdatePassword(new int(), new UpdatePasswordRequest());
            mockLogger.Verify(l => l.Log<It.IsAnyType>(
                LogLevel.Information, It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                null,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task UpdatePassword_Calls_UserService_UpdateUser()
        {
            var userId = 1;
            var userPassword = "Admin1!"; // password used for default Admin account
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            await usersController.UpdatePassword(userId, new UpdatePasswordRequest { Password = userPassword });
            mockUserService.Verify(s => s.UpdateUserPasswordAsync(userId, userPassword), Times.Once);
        }

        [Test]
        public async Task UpdatePassword_Maps_User_To_UserResource()
        {
            var resource = new User();
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.UpdateUserPasswordAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(resource);

            await usersController.UpdatePassword(new int(), new UpdatePasswordRequest());

            mockMapper.Verify(m => m.Map<UserResource>(It.Is<User>(r => r == resource)));
        }

        [Test]
        public async Task UpdatePassword_Returns_BadRequestObjectResult_With_PasswordValidationResult_If_Password_Invalid()
        {
            var password = "test";
            var error = "test error";
            var validationResult = new PasswordValidationResult();
            validationResult.Errors.Add(error);

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(validationResult);
            var request = new UpdatePasswordRequest { Password = password };
            var result = await usersController.UpdatePassword(new int(), request) as BadRequestObjectResult;
            var actual = result.Value as PasswordValidationResult;

            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(validationResult, actual);
        }

        [Test]
        public async Task UpdatePassword_Returns_OkObjectResult_If_User_Found()
        {
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.UpdateUserPasswordAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new User());

            var result = await usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task UpdatePassword_Returns_NotFoundResult_If_User_Not_Found()
        {
            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            var result = await usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as NotFoundResult;

            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task UpdatePassword_Returns_UserResource()
        {
            var userId = 1;
            var resource = new UserResource { Id = userId };

            mockUserPasswordValidationService.Setup(s => s.Validate(It.IsAny<string>())).Returns(new PasswordValidationResult());
            mockUserService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new User());
            mockUserService.Setup(s => s.UpdateUserPasswordAsync(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new User());
            mockMapper.Setup(m => m.Map<UserResource>(It.IsAny<User>())).Returns(resource);

            var result = await usersController.UpdatePassword(new int(), new UpdatePasswordRequest()) as OkObjectResult;
            var actual = result.Value as UserResource;

            Assert.AreEqual(actual, resource);
        }
    }
}
