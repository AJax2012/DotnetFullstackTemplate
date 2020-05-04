using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SourceName.Api.Model.User
{
    public class UpdatePasswordRequest
    {
        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters in length")]
        public string Password { get; set; }
    }
}
