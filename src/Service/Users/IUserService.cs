using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SourceName.Data.Model;
using SourceName.Service.Model.Users;

namespace SourceName.Service.Users
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<PaginatedResult<User>> GetAllPaginatedAsync(int pageNumber = 0, int resultsPerPage = 0, bool removeInactive = true);
        Task<User> GetByIdAsync(int id);
        Task<User> GetByIdWithRolesAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetForAuthenticationAsync(string username);
        Task<User> UpdateUserAsync(User user);
        Task<User> UpdateUserPasswordAsync(int? id, string password);
        Task<User> UpdateUserRolesAsync(int id, List<int> roles);
    }
}