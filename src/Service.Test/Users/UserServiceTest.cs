using AutoMapper;
using Moq;
using NUnit.Framework;
using SourceName.Data.Model.User;
using SourceName.Data.Users;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SourceName.Data.Model;
using SourceName.Data.Model.Role;
using SourceName.Service.Model;

namespace SourceName.Service.Implementation.Test.Users
{
    [TestFixture]
    public class UserServiceTest
    {
        private Mock<IMapper> mockMapper;
        private Mock<IUserPasswordService> mockUserPasswordService;
        private Mock<IUserRepository> mockUserRepository;
        private IUserService userService;

        [SetUp]
        public void SetUp()
        {
            mockMapper = new Mock<IMapper>();
            mockUserPasswordService = new Mock<IUserPasswordService>();
            mockUserRepository = new Mock<IUserRepository>();
            userService = new UserService(mockMapper.Object, mockUserPasswordService.Object, mockUserRepository.Object);
        }

        [Test]
        public async Task CreateUser_Calls_UserPasswordService_CreateHash()
        {
            var password = "Admin1!"; // password used for default admin account
            byte[] passwordHash;
            byte[] passwordSalt;

            await userService.CreateUserAsync(new User { Password = password });
            mockUserPasswordService.Verify(r => r.CreateHash(It.Is<string>(p => p == password), out passwordHash, out passwordSalt), Times.Once);
        }

        [Test]
        public async Task CreateUser_Calls_UserRepository_Insert()
        {
            var userName = "test";
            var user = new User { Username = userName };
            await userService.CreateUserAsync(user);
            mockUserRepository.Verify(r => r.InsertAsync(It.Is<UserEntity>(ue => ue.Username == userName)), Times.Once);
        }

        [Test]
        public async Task CreateUser_Maps_UserEntity_To_User()
        {
            var userName = "test";
            var userEntity = new UserEntity { Username = userName };

            mockUserRepository.Setup(r => r.InsertAsync(It.IsAny<UserEntity>())).ReturnsAsync(userEntity);

            await userService.CreateUserAsync(new User());
            mockMapper.Verify(m => m.Map<User>(It.Is<UserEntity>(ue => ue.Username == userName)), Times.Once);
        }

        [Test]
        public async Task CreateUser_Returns_User()
        {
            var resource = new User();

            mockUserRepository.Setup(r => r.InsertAsync(It.IsAny<UserEntity>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(resource);

            var result = await userService.CreateUserAsync(new User());

            Assert.AreEqual(resource, result);
        }

        [Test]
        public async Task DeleteUser_Calls_UserRepository_Delete()
        {
            var userId = 1;
            await userService.DeleteUserAsync(userId);
            mockUserRepository.Verify(r => r.DeleteAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetAll_Calls_UserRepository_Get()
        {
            await userService.GetAllPaginatedAsync();
            mockUserRepository.Verify(r => r.GetPaginatedEntitiesAsync(It.IsAny<PagingatedQuery<UserEntity>>()), Times.Once);
        }

        [Test]
        public async Task GetAll_Maps_List_UserEntity_To_List_User()
        {
            var user = new UserEntity();
            var userList = new List<UserEntity> { user };
            var expectedResult = new PaginatedResult<UserEntity> { Data = userList, TotalCount = userList.Count };

            mockUserRepository.Setup(r => r.GetPaginatedEntitiesAsync(It.IsAny<PagingatedQuery<UserEntity>>())).ReturnsAsync(expectedResult);
            await userService.GetAllPaginatedAsync();
            mockMapper.Verify(r => r.Map<SearchResult<User>>(It.Is<PaginatedResult<UserEntity>>(list => list.Data.First() == user)), Times.Once);
        }

        [Test]
        public async Task GetAll_Returns_Expected_User_List()
        {
            var userList = new List<User> { new User() };
            var expectedResult = new SearchResult<User> { Data = userList, TotalCount = userList.Count };

            mockUserRepository.Setup(r => r.GetPaginatedEntitiesAsync(It.IsAny<PagingatedQuery<UserEntity>>())).ReturnsAsync(new PaginatedResult<UserEntity>());
            mockMapper.Setup(m => m.Map<SearchResult<User>>(It.IsAny<PaginatedResult<UserEntity>>())).Returns(expectedResult);

            var result = await userService.GetAllPaginatedAsync();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Data);
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public async Task GetById_Calls_UserRepository_GetById()
        {
            var userId = 1;
            await userService.GetByIdAsync(userId);
            mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetById_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(userEntity);
            await userService.GetByIdAsync(new int());
            mockMapper.Verify(r => r.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public async Task GetById_Returns_NotEmpty_User()
        {
            var userId = 1;

            mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(new User());

            var result = await userService.GetByIdAsync(userId);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetById_Returns_Expected_User()
        {
            var userId = 1;
            var resource = new User { Id = userId };

            mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(resource);

            var result = await userService.GetByIdAsync(userId);

            Assert.AreEqual(resource, result);
        }

        [Test]
        public async Task GetByUsername_Calls_UserRepository_Get()
        {
            var username = "test";

            await userService.GetByUsernameAsync(username);
            mockUserRepository.Verify(r => r.GetEntityFirstOrDefaultAsync(It.IsAny<Query<UserEntity>>()), Times.Once);
        }

        [Test]
        public async Task GetByUsername_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.GetEntityFirstOrDefaultAsync(It.IsAny<Query<UserEntity>>())).ReturnsAsync(userEntity);
            await userService.GetByUsernameAsync("test");
            mockMapper.Verify(m => m.Map<User>(userEntity));
        }

        [Test]
        public async Task GetByUsername_Returns_User()
        {
            var user = new User();
            mockUserRepository.Setup(r => r.GetEntityFirstOrDefaultAsync(It.IsAny<Query<UserEntity>>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);
            var result = await userService.GetByUsernameAsync("test");

            Assert.NotNull(result);
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task GetForAuthentication_Calls_UserRepository_GetByUsernameWithRoles()
        {
            var username = "test";
            await userService.GetForAuthenticationAsync(username);
            mockUserRepository.Verify(r => r.GetByUsernameWithRolesAsync(username), Times.Once);
        }

        [Test]
        public async Task GetForAuthentication_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.GetByUsernameWithRolesAsync(It.IsAny<string>())).ReturnsAsync(userEntity);

            await userService.GetForAuthenticationAsync("test");
            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public async Task GetForAuthentication_Returns_User()
        {
            var user = new User();
            mockUserRepository.Setup(r => r.GetByUsernameWithRolesAsync(It.IsAny<string>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = await userService.GetForAuthenticationAsync("test");

            Assert.IsNotNull(result);
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task UpdateUser_Maps_User_To_UserEntity()
        {
            var user = new User();

            mockMapper.Setup(r => r.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());

            await userService.UpdateUserAsync(user);
            mockMapper.Verify(m => m.Map<UserEntity>(user), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Calls_UserRepository_Update()
        {
            var userEntity = new UserEntity();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(userEntity);

            await userService.UpdateUserAsync(new User());
            mockUserRepository.Verify(r => r.UpdateAsync(userEntity), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());
            mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(userEntity);

            await userService.UpdateUserAsync(new User());
            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public async Task UpdateUser_Returns_User()
        {
            var user = new User();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());
            mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = await userService.UpdateUserAsync(new User());

            Assert.IsNotNull(result);
            Assert.AreEqual(user, result);
        }

        [Test]
        public async Task UpdateUserRoles_Calls_UserRepository_GetByIdWithRoles_Twice()
        {
            var userId = 1;
            var userRoles = new List<int> { 1 };
            var userEntity = new UserEntity {Id = userId };

            mockUserRepository.Setup(r => r.GetByIdWithRolesAsync(It.IsAny<int>())).ReturnsAsync(userEntity);

            await userService.UpdateUserRolesAsync(userId, userRoles);
            mockUserRepository.Verify(r => r.GetByIdWithRolesAsync(userId), Times.Exactly(2));
        }

        [Test]
        public async Task UpdateUserRoles_Calls_UserRepository_AddUserRoles()
        {
            var userId = 1;
            var curretUserRoleId = 1;
            var currentUserRoles = new List<UserRoleEntity>
            {
                new UserRoleEntity
                {
                    Id = 1,
                    RoleId = curretUserRoleId,
                    UserId = userId
                }
            };

            var roleIdToAdd = 2;
            var rolesParameter = new List<int> { curretUserRoleId, roleIdToAdd };
            var userEntity = new UserEntity { Id = userId, Roles = currentUserRoles };

            mockUserRepository.Setup(r => r.GetByIdWithRolesAsync(It.IsAny<int>())).ReturnsAsync(userEntity);

            await userService.UpdateUserRolesAsync(userId, rolesParameter);
            mockUserRepository.Verify(r => r.AddUserRolesAsync(
                It.Is<IEnumerable<UserRoleEntity>>(ur => ur.First().RoleId == roleIdToAdd &&
                                                         ur.First().UserId == userId)), Times.Once);
        }

        [Test]
        public async Task UpdateUserRoles_Calls_UserRepository_RemoveUserRoles()
        {
            var userId = 1;
            var curretUserRoleId = 1;
            var roleIdToRemove = 2;
            var currentUserRoles = new List<UserRoleEntity>
            {
                new UserRoleEntity
                {
                    Id = 1,
                    RoleId = curretUserRoleId,
                    UserId = userId
                },
                new UserRoleEntity
                {
                    Id = 2,
                    RoleId = roleIdToRemove,
                    UserId = userId
                }
            };

            var rolesParameter = new List<int> { curretUserRoleId };
            var userEntity = new UserEntity { Id = userId, Roles = currentUserRoles };

            mockUserRepository.Setup(r => r.GetByIdWithRolesAsync(It.IsAny<int>())).ReturnsAsync(userEntity);

            await userService.UpdateUserRolesAsync(userId, rolesParameter);
            mockUserRepository.Verify(r => r.RemoveUserRolesAsync(
                It.Is<IEnumerable<UserRoleEntity>>(ur => ur.First().RoleId == roleIdToRemove &&
                                                         ur.First().UserId == userId)), Times.Once);
        }

        [Test]
        public async Task UpdateUserRoles_Maps_UserEntity_To_User()
        {
            var userId = 1;
            var roles = new List<int> { 1 };
            var userEntity = new UserEntity { Id = userId };

            mockUserRepository.Setup(r => r.GetByIdWithRolesAsync(It.IsAny<int>())).ReturnsAsync(userEntity);

            await userService.UpdateUserRolesAsync(userId, roles);
            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public async Task UpdateUserRoles_Returns_User()
        {
            var userId = 1;
            var userRoles = new List<int> { 1 };
            var userEntity = new UserEntity { Id = userId };
            var expectedUser = new User { Id = userId };

            mockUserRepository.Setup(r => r.GetByIdWithRolesAsync(It.IsAny<int>())).ReturnsAsync(userEntity);
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(expectedUser);

            var result = await userService.UpdateUserRolesAsync(userId, userRoles);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedUser, result);
        }

        [Test]
        public async Task UpdateUserPassword_Calls_UserPasswordService_CreateHash()
        {
            var id = 1;
            var password = "Admin1!"; // password for default admin account
            byte[] passwordHash;
            byte[] passwordSalt;

            await userService.UpdateUserPasswordAsync(id, password);
            mockUserPasswordService.Verify(s => s.CreateHash(password, out passwordHash, out passwordSalt), Times.Once);
        }

        [Test]
        public async Task UpdateUserPassword_Calls_UserRepository_UpdatePassword()
        {
            var id = 1;
            byte[] passwordHash = Encoding.UTF8.GetBytes("TestHash");
            byte[] passwordSalt = Encoding.UTF8.GetBytes("TestSalt");

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            await userService.UpdateUserPasswordAsync(id, "Test");
            mockUserRepository.Verify(s => s.UpdatePasswordAsync(id, passwordHash, passwordSalt), Times.Once);
        }

        [Test]
        public async Task UpdateUserPassword_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            byte[] passwordHash;
            byte[] passwordSalt;

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            mockUserRepository.Setup(r => r.UpdatePasswordAsync(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(userEntity);

            await userService.UpdateUserPasswordAsync(new int(), "test");

            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public async Task UpdateUserPassword_Returns_User()
        {
            var user= new User();
            byte[] passwordHash;
            byte[] passwordSalt;

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            mockUserRepository.Setup(r => r.UpdatePasswordAsync(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).ReturnsAsync(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = await userService.UpdateUserPasswordAsync(new int(), "test");

            Assert.IsNotNull(result);
            Assert.AreEqual(result, user);
        }
    }
}
