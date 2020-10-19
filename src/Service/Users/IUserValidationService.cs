using SourceName.Service.Model.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SourceName.Service.Users
{
    public interface IUserValidationService
    {
        public Task<UserValidationResult> ValidateUserAsync(User user);
    }
}
