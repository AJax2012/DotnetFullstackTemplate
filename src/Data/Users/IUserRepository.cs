using System;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.User;

namespace SourceName.Data.Users
{
    public interface IUserRepository : IRepository<UserEntity>, IIntegerRepository<UserEntity>
    {
        UserEntity GetByUsernameWithRoles(string username);
        UserEntity UpdatePassword(int? id, byte[] passwordHash, byte[] passwordSalt);
    }
}