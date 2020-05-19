using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceName.Service.Model.Users
{
    public class UserValidationResult
    {
        public List<string> Errors { get; } = new List<string>();
        public bool IsValid => !Errors.Any();
    }
}
