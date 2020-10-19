using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using SourceName.Data.Model;
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

        public async Task<User> CreateUserAsync(User user)
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

            var result = await _userRepository.InsertAsync(userEntity);
            return _mapper.Map<User>(result);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<PaginatedResult<User>> GetAllPaginatedAsync(int pageNumber = 0, int resultsPerPage = 10, bool removeInactive = true)
        {
            Expression<Func<UserEntity, bool>> where = null;
            Expression<Func<UserEntity, object>> include = u => u.Roles;

            if (removeInactive)
            {
                where = u => u.IsActive == true;
            }

            var query = new PagingatedQuery<UserEntity>(pageNumber, resultsPerPage)
            {
                Where = where,
                OrderPrimary = u => u.LastName,
                OrderSecondary = u => u.FirstName,
                IncludeProperties = new List<Expression<Func<UserEntity, object>>> { include }
            };

            var userEntities = await _userRepository.GetPaginatedEntitiesAsync(query);

            return _mapper.Map<PaginatedResult<User>>(userEntities);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            var userEntity = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetByIdWithRolesAsync(int id)
        {
            var userEntity = await _userRepository.GetByIdWithRolesAsync(id);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            Expression<Func<UserEntity, object>> include = x => x.Roles;

            var query = new Query<UserEntity>
            {
                Where = x => x.Username == username,
                IncludeProperties = new List<Expression<Func<UserEntity, object>>> { include }
            };

            var userEntity = await _userRepository
                .GetEntityFirstOrDefaultAsync(query);

            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> GetForAuthenticationAsync(string username)
        {
            var userEntity = await _userRepository.GetByUsernameWithRolesAsync(username);
            return _mapper.Map<User>(userEntity);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var userEntity = _mapper.Map<UserEntity>(user);

            return _mapper.Map<User>(await _userRepository.UpdateAsync(userEntity));
        }

        public async Task<User> UpdateUserRolesAsync(int id, List<int> roleIds)
        {
            var userEntity = await _userRepository.GetByIdWithRolesAsync(id);

            if (userEntity.Roles != null)
            {
                var rolesToAdd = roleIds.Where(
                newRole =>
                    !userEntity.Roles.Any(existingRole => existingRole.RoleId == newRole))
                .Select(r => new UserRoleEntity
                {
                    RoleId = r,
                    UserId = userEntity.Id,
                });

                if (rolesToAdd.Any())
                {
                    await _userRepository.AddUserRolesAsync(rolesToAdd);
                }
            }

            var rolesToRemove = userEntity.Roles?.Where(
                existingRole =>
                    !roleIds.Any(roleId => existingRole.RoleId == roleId));

            if (rolesToRemove != null && rolesToRemove.Any())
            {
                await _userRepository.RemoveUserRolesAsync(rolesToRemove);
            }

            return _mapper.Map<User>(await _userRepository.GetByIdWithRolesAsync(id));
        }

        public async Task<User> UpdateUserPasswordAsync(int? id, string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            _userPasswordService.CreateHash(password, out passwordHash, out passwordSalt);
            var userEntity = await _userRepository.UpdatePasswordAsync(id, passwordHash, passwordSalt);
            return _mapper.Map<User>(userEntity);
        }
    }
}