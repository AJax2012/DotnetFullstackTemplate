using System;
using System.Collections.Generic;
using System.Linq;

namespace SourceName.Service.Model.Users
{
    public class PasswordValidationResult
    {
        public List<string> Errors { get; } = new List<string>();
        public bool IsValid => !Errors.Any();
    }
}
