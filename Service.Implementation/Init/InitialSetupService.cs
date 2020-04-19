using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.Role;
using SourceName.Data.Roles;
using SourceName.Service.Init;
using SourceName.Service.Model.Roles;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Init
{
    public class InitialSetupService : IInitialSetupService
    {
        private readonly User _adminUser;
        private readonly IList<Role> _roles = new List<Role>();
        private readonly IUserService _userService;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public InitialSetupService(IUserService userService,
                                   IRoleRepository roleRepository,
                                   IMapper mapper)
        {
            var roleNames = Enum.GetValues(typeof(Roles));
            foreach (var role in roleNames)
            {
                _roles.Add(new Role
                {
                    Id = (int)role,
                    Name = role.ToString(),
                    Description = role.ToString()
                });
            }

            _adminUser = new User
            {
                FirstName = "Default",
                LastName = "Administrator",
                Username = "admin",
                Password = "Admin1!",
                IsActive = true,
            };

            _adminUser.Roles.Add(new UserRole
            {
                RoleId = (int)Roles.Administrator
            });

            _userService = userService;
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public void InitialSetup()
        {
            if (_roleRepository.Get().ToList().Any())
            {
                return;
            }

            //TODO: Eventually, log if the amount of Roles in the DB != amount of roles in Roles Enum

            foreach (var role in _roles)
            {
                _roleRepository.Insert(_mapper.Map<RoleEntity>(role));
            }

            _userService.CreateUser(_adminUser);
        }
    }
}