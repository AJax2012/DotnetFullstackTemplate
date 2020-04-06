using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;
using SourceName.Data.Users;
using SourceName.Service.Model.Users;
using SourceName.Service.Users;

namespace SourceName.Service.Implementation.Users
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserPasswordService _userPasswordService;
        private readonly IUserRepository _userRepository;

        public UserService(IMapper mapper, IUserPasswordService userPasswordService, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userPasswordService = userPasswordService;
            _userRepository = userRepository;
        }

        public User CreateUser(User user)
        {
            byte[] passwordHash;
            byte[] passwordSalt;
            _userPasswordService.CreateHash(user.Password, out passwordHash, out passwordSalt);

            var userEntity = new UserEntity
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true
            };

            userEntity.Roles = user.Roles.Select(r =>
                new UserRoleEntity
                {
                    UserId = userEntity.Id,
                    RoleId = r.RoleId
                }).ToList();

            var result = _userRepository.Insert(userEntity);
            return _mapper.Map<User>(result);
        }

        public void DeleteUser(Guid id)
        {
            _userRepository.Delete(id);
        }

        public List<User> GetAll()
        {
            var userEntities = _userRepository.Get()
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName);

            return _mapper.Map<List<User>>(userEntities);
        }

        public User GetById(Guid id)
        {
            var userEntity = _userRepository.GetById(id);
            return _mapper.Map<User>(userEntity);
        }

        public User GetByUsername(string username)
        {
            var userEntity = _userRepository
                .Get(x => x.Username == username)
                .SingleOrDefault();

            return _mapper.Map<User>(userEntity);
        }

        public User GetForAuthentication(string username)
        {
            var userEntity = _userRepository.GetByUsernameWithRoles(username);
            return _mapper.Map<User>(userEntity);
        }

        public User UpdateUser(User user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);
            userEntity.Roles = user.Roles.Select(role => new UserRoleEntity
            {
                UserId = user.Id,
                RoleId = role.RoleId
            }).ToList();

            return _mapper.Map<User>(_userRepository.Update(userEntity));
        }
    }
}