using System;
using System.Collections.Generic;
using SourceName.Service.Model.Users;

namespace SourceName.Service.Users
{
    public interface IUserService
    {
        User CreateUser(User user);
        void DeleteUser(Guid id);
        List<User> GetAll();
        User GetById(Guid id);
        User GetByUsername(string username);
        User GetForAuthentication(string username);
        User UpdateUser(User user);
    }
}