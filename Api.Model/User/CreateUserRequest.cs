using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SourceName.Api.Model.User
{
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters in length")]
        public string Password { get; set; }

        [Required] 
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
