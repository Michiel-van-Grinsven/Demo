using System.ComponentModel.DataAnnotations;

namespace UsingAuthorizationWithSwagger.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
    }
}
