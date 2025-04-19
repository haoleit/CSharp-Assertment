
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser> RegisterUserAsync(string username, string email, string password);
        Task<ApplicationUser> LoginUserAsync(string email, string password);
        Task<ApplicationUser> GetUserByUsernameAsync(string username);
        Task<bool> AddRoleToUserAsync(ApplicationUser user, string role);
    }
}
