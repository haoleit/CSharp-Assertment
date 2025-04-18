
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repositories;

namespace TaskManagerAPI.Services.impl
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var user = await _authRepository.RegisterUserAsync(registerDto.Username, registerDto.Email, registerDto.Password);
            Console.WriteLine("HELLEO " + user);
            if (user == null)
            {
                return "Registration failed.";
            }

            // Thêm vai trò mặc định cho người dùng (User)
            await _authRepository.AddRoleToUserAsync(user, "User");

            return "User registered successfully.";
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _authRepository.LoginUserAsync(loginDto.Email, loginDto.Password);
            if (user == null)
            {
                return "Invalid credentials.";
            }

            var token = GenerateJwtToken(user);
            return token;
        }

        public async Task<string> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            var user = await _authRepository.GetUserByUsernameAsync(assignRoleDto.Username);
            if (user == null)
            {
                return "User not found.";
            }

            bool result = await _authRepository.AddRoleToUserAsync(user, assignRoleDto.Role);
            return result ? $"Role {assignRoleDto.Role} assigned successfully" : "Failed to assign role.";
        }

        private string GenerateJwtToken(ApplicationUser user)
        {


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
