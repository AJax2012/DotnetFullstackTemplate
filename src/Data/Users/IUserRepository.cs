using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.Role;
using SourceName.Data.Model.User;

namespace SourceName.Data.Users
{
    public interface IUserRepository : IRepository<UserEntity>, IIntegerRepository<UserEntity>
    {
        Task AddUserRolesAsync(IEnumerable<UserRoleEntity> rolesToAdd);
        Task<UserEntity> GetByIdWithRolesAsync(int id);
        Task<UserEntity> GetByUsernameWithRolesAsync(string username);
        Task RemoveUserRolesAsync(IEnumerable<UserRoleEntity> rolesToRemove);
        Task<UserEntity> UpdatePasswordAsync(int? id, byte[] passwordHash, byte[] passwordSalt);
    }
}