using AutoMapper;
using Moq;
using NUnit.Framework;
using SourceName.Data.Model.User;
using SourceName.Data.Users;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SourceName.Service.Model.Roles;

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
        public void CreateUser_Calls_UserPasswordService_CreateHash()
        {
            var password = "Admin1!"; // password used for default admin account
            byte[] passwordHash;
            byte[] passwordSalt;

            userService.CreateUser(new User { Password = password });
            mockUserPasswordService.Verify(r => r.CreateHash(It.Is<string>(p => p == password), out passwordHash, out passwordSalt), Times.Once);
        }

        [Test]
        public void CreateUser_Calls_UserRepository_Insert()
        {
            var userName = "test";
            var user = new User { Username = userName };
            userService.CreateUser(user);
            mockUserRepository.Verify(r => r.Insert(It.Is<UserEntity>(ue => ue.Username == userName)), Times.Once);
        }

        [Test]
        public void CreateUser_Maps_UserEntity_To_User()
        {
            var userName = "test";
            var userEntity = new UserEntity { Username = userName };

            mockUserRepository.Setup(r => r.Insert(It.IsAny<UserEntity>())).Returns(userEntity);

            userService.CreateUser(new User());
            mockMapper.Verify(m => m.Map<User>(It.Is<UserEntity>(ue => ue.Username == userName)), Times.Once);
        }

        [Test]
        public void CreateUser_Returns_User()
        {
            var resource = new User();

            mockUserRepository.Setup(r => r.Insert(It.IsAny<UserEntity>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(resource);

            var result = userService.CreateUser(new User());

            Assert.AreEqual(result, resource);
        }

        [Test]
        public void DeleteUser_Calls_UserRepository_Delete()
        {
            var userId = 1;
            userService.DeleteUser(userId);
            mockUserRepository.Verify(r => r.Delete(userId), Times.Once);
        }

        [Test]
        public void GetAll_Calls_UserRepository_Get()
        {
            userService.GetAll();
            mockUserRepository.Verify(r => r.Get(null), Times.Once);
        }

        [Test]
        public void GetAll_Maps_List_UserEntity_To_List_User()
        {
            var user = new UserEntity();
            mockUserRepository.Setup(r => r.Get(It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity> { user });
            userService.GetAll();
            mockMapper.Verify(r => r.Map<List<User>>(It.Is<IOrderedEnumerable<UserEntity>>(list => list.First() == user)), Times.Once);
        }

        [Test]
        public void GetAll_Returns_NotEmpty_User_List()
        {
            mockUserRepository.Setup(r => r.Get(It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity> { new UserEntity() });
            mockMapper.Setup(m => m.Map<IEnumerable<User>>(It.IsAny<IEnumerable<UserEntity>>())).Returns(new List<User> { new User() });
            
            var result = userService.GetAll();

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void GetAll_Returns_Expected_User_List()
        {
            var resource = new List<User> { new User() };

            mockUserRepository.Setup(r => r.Get(It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity> { new UserEntity() });
            mockMapper.Setup(m => m.Map<IEnumerable<User>>(It.IsAny<IEnumerable<UserEntity>>())).Returns(resource);

            var result = userService.GetAll();

            Assert.AreEqual(result, resource);
        }

        [Test]
        public void GetById_Calls_UserRepository_GetById()
        {
            var userId = 1;
            userService.GetById(userId);
            mockUserRepository.Verify(r => r.GetById(userId), Times.Once);
        }

        [Test]
        public void GetById_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns(userEntity);
            userService.GetById(new int());
            mockMapper.Verify(r => r.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public void GetById_Returns_NotEmpty_User()
        {
            var userId = 1;

            mockUserRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(new User());

            var result = userService.GetById(userId);

            Assert.IsNotNull(result);
        }

        [Test]
        public void GetById_Returns_Expected_User()
        {
            var userId = 1;
            var resource = new User { Id = userId };

            mockUserRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(resource);

            var result = userService.GetById(userId);

            Assert.AreEqual(result, resource);
        }

        [Test]
        public void GetByUsername_Calls_UserRepository_Get()
        {
            var username = "test";
            userService.GetByUsername(username);
            mockUserRepository.Verify(r => r.Get(x => x.Username == username));
        }

        [Test]
        public void GetByUsername_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.Get(It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity> { userEntity });
            userService.GetByUsername("test");
            mockMapper.Verify(m => m.Map<User>(userEntity));
        }

        [Test]
        public void GetByUsername_Returns_User()
        {
            var user = new User();
            mockUserRepository.Setup(r => r.Get(It.IsAny<Expression<Func<UserEntity, bool>>>())).Returns(new List<UserEntity>());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);
            var result = userService.GetByUsername("test");

            Assert.NotNull(result);
            Assert.AreEqual(result, user);
        }

        [Test]
        public void GetForAuthentication_Calls_UserRepository_GetByUsernameWithRoles()
        {
            var username = "test";
            userService.GetForAuthentication(username);
            mockUserRepository.Verify(r => r.GetByUsernameWithRoles(username), Times.Once);
        }

        [Test]
        public void GetForAuthentication_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            mockUserRepository.Setup(r => r.GetByUsernameWithRoles(It.IsAny<string>())).Returns(userEntity);

            userService.GetForAuthentication("test");
            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public void GetForAuthentication_Returns_User()
        {
            var user = new User();
            mockUserRepository.Setup(r => r.GetByUsernameWithRoles(It.IsAny<string>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = userService.GetForAuthentication("test");

            Assert.IsNotNull(result);
            Assert.AreEqual(result, user);
        }

        [Test]
        public void UpdateUser_Maps_User_To_UserEntity()
        {
            var user = new User();

            mockMapper.Setup(r => r.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());

            userService.UpdateUser(user);
            mockMapper.Verify(m => m.Map<UserEntity>(user), Times.Once);
        }

        [Test]
        public void UpdateUser_Calls_UserRepository_Update()
        {
            var userEntity = new UserEntity();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(userEntity);

            userService.UpdateUser(new User());
            mockUserRepository.Verify(r => r.Update(userEntity), Times.Once);
        }

        [Test]
        public void UpdateUser_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());
            mockUserRepository.Setup(r => r.Update(It.IsAny<UserEntity>())).Returns(userEntity);

            userService.UpdateUser(new User());
            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public void UpdateUser_Returns_User()
        {
            var user = new User();

            mockMapper.Setup(m => m.Map<UserEntity>(It.IsAny<User>())).Returns(new UserEntity());
            mockUserRepository.Setup(r => r.Update(It.IsAny<UserEntity>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = userService.UpdateUser(new User());

            Assert.IsNotNull(result);
            Assert.AreEqual(result, user);
        }

        [Test]
        public void UpdateUserPassword_Calls_UserPasswordService_CreateHash()
        {
            var id = 1;
            var password = "Admin1!"; // password for default admin account
            byte[] passwordHash;
            byte[] passwordSalt;

            userService.UpdateUserPassword(id, password);
            mockUserPasswordService.Verify(s => s.CreateHash(password, out passwordHash, out passwordSalt), Times.Once);
        }

        [Test]
        public void UpdateUserPassword_Calls_UserRepository_UpdatePassword()
        {
            var id = 1;
            byte[] passwordHash = Encoding.UTF8.GetBytes("TestHash");
            byte[] passwordSalt = Encoding.UTF8.GetBytes("TestSalt");

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            userService.UpdateUserPassword(id, "Test");
            mockUserRepository.Verify(s => s.UpdatePassword(id, passwordHash, passwordSalt), Times.Once);
        }

        [Test]
        public void UpdateUserPassword_Maps_UserEntity_To_User()
        {
            var userEntity = new UserEntity();
            byte[] passwordHash;
            byte[] passwordSalt;

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            mockUserRepository.Setup(r => r.UpdatePassword(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(userEntity);

            userService.UpdateUserPassword(new int(), "test");

            mockMapper.Verify(m => m.Map<User>(userEntity), Times.Once);
        }

        [Test]
        public void UpdateUserPassword_Returns_User()
        {
            var user= new User();
            byte[] passwordHash;
            byte[] passwordSalt;

            mockUserPasswordService.Setup(s => s.CreateHash(It.IsAny<string>(), out passwordHash, out passwordSalt));
            mockUserRepository.Setup(r => r.UpdatePassword(It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(new UserEntity());
            mockMapper.Setup(m => m.Map<User>(It.IsAny<UserEntity>())).Returns(user);

            var result = userService.UpdateUserPassword(new int(), "test");

            Assert.IsNotNull(result);
            Assert.AreEqual(result, user);
        }
    }
}
