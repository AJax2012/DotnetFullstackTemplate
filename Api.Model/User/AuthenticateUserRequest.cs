using System.ComponentModel.DataAnnotations;

namespace SourceName.Api.Model.User
{
    public class AuthenticateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }
    }
}
