
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
