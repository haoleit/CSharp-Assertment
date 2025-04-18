
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class AssignRoleDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
