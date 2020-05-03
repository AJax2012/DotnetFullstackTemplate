using Moq;
using NUnit.Framework;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Model.Roles;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Service.Implementation.Test
{
    [TestFixture]
    public class UserCapabilitiesServiceTest
    {
        private Mock<IUserService> mockUserService;
        private IUserCapabilitiesService userCapabilitiesService;

        [SetUp]
        public void SetUp()
        {
            mockUserService = new Mock<IUserService>();
            userCapabilitiesService = new UserCapabilitiesService(mockUserService.Object);
        }

        [Test]
        public void GetUserCapabilities_Calls_UserService_GetById()
        {
            var userId = 1;
            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(new User());
            userCapabilitiesService.GetUserCapabilities(userId);
            mockUserService.Verify(s => s.GetById(userId), Times.Once);
        }

        /*
         * Please test each Roles/Role
         * when a new role type is created
         */
        [Test]
        [TestCase(Roles.Administrator, true)]
        [TestCase(Roles.User, false)]
        public void GetUserCapabilities_Returns_UserCapabilities_With_Expected_CanManageUsers_Property(Roles role, bool expectedResult)
        {
            var user = new User 
            {
                Roles = new List<UserRole> { new UserRole { RoleId = (int)role } }
            };

            mockUserService.Setup(s => s.GetById(It.IsAny<int>())).Returns(user);

            var actualResult = userCapabilitiesService.GetUserCapabilities(new int());

            Assert.IsNotNull(actualResult);
            Assert.AreEqual(actualResult.CanManageUsers, expectedResult);
        }
    }
}
