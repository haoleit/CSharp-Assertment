
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var user = await _authRepository.RegisterUserAsync(registerDto.Username, registerDto.Email, registerDto.Password);
            Console.WriteLine("HELLEO " + user);
            if (user == null)
            {
                return "Registration failed.";
            }


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

            var response = _httpContextAccessor.HttpContext.Response;
            response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)
            });

            return "Logged in successfully.";
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

        private ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);



                return principal;
            }
            catch
            {
                return null;
            }
        }


        public async Task<ApplicationUser?> GetProfileAsync()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            // Validate the token
            var claimsPrincipal = ValidateJwtToken(token);
            if (claimsPrincipal == null)
            {
                return null;
            }


            var username = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var user = await _authRepository.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<string> LogOutAsync()
        {

            var response = _httpContextAccessor.HttpContext.Response;
            response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return "User logged out successfully";
        }



    }






}
