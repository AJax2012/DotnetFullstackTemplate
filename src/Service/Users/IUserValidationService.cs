using SourceName.Service.Model.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceName.Service.Users
{
    public interface IUserValidationService
    {
        public UserValidationResult ValidateUser(User user);
    }
}
