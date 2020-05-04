using System.ComponentModel.DataAnnotations;

namespace SourceName.Api.Model.User
{
    public class AuthenticateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters in length")]
        public string Password { get; set; }
    }
}
