using System.ComponentModel.DataAnnotations;

namespace TaskAPI.DTO
{
    public class LoginUserDto
    {
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
