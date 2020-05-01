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

namespace SourceName.Service.Implementation.Test
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
            mockUserRepository.Verify(r => r.Delete(It.Is<int>(id => id == userId)), Times.Once);
        }

        [Test]
        public void GetAll_Calls_UserRepository_Get()
        {
            userService.GetAll();
            mockUserRepository.Verify(r => r.Get(It.IsAny<Expression<Func<UserEntity,bool>>>()), Times.Once);
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
            mockUserRepository.Verify(r => r.GetById(It.Is<int>(id => id == userId)), Times.Once);
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
    }
}
