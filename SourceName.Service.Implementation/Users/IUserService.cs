using System.Collections.Generic;
using SourceName.Service.Model.Users;

namespace SourceName.Service.Implementation.Users
{
    public interface IUserService
    {
        User CreateUser(User user);
        void DeleteUser(int id);
        List<User> GetAll();
        User GetById(int id);
        User GetByUsername(string username);
        User GetForAuthentication(string username);
        User UpdateUser(User user);
    }
}