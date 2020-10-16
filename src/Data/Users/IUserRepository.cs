using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.User;

namespace SourceName.Data.Users
{
    public interface IUserRepository : IRepository<UserEntity>, IIntegerRepository<UserEntity>
    {
        IEnumerable<UserEntity> Get(Expression<Func<UserEntity, bool>> filter = null);
        UserEntity GetByUsernameWithRoles(string username);
        UserEntity UpdatePassword(int? id, byte[] passwordHash, byte[] passwordSalt);
    }
}