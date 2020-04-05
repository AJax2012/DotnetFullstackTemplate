using SourceName.Data.GenericRepositories;
using SourceName.Data.Model.User;

namespace SourceName.Data.Users
{
    public interface IUserRepository : IRepository<UserEntity>, IGuidRepository<UserEntity>
    {
        UserEntity GetByUsernameWithRoles(string username);
    }
}