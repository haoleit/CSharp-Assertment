

using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<string> AssignRoleAsync(AssignRoleDto assignRoleDto);

        Task<ApplicationUser> GetProfileAsync();
        Task<string> LogOutAsync();
    }
}
