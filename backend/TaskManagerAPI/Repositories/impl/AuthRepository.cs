
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repositories.impl
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthRepository(DataContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> RegisterUserAsync(string username, string email, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> AddRoleToUserAsync(ApplicationUser user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
    }
}
