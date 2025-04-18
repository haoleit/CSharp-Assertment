// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;

using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (result == "User registered successfully.")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            if (token == "Invalid credentials.")
            {
                return Unauthorized(new { message = token });
            }

            return Ok(new { token });
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _authService.AssignRoleAsync(assignRoleDto);
            if (result.Contains("assigned"))
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { message = result });
        }

        [HttpGet("session")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _authService.GetProfileAsync();
            if (user == null)
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            return Ok(new { userName = user.UserName, email = user.Email });  // Return basic user info
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogOutAsync();
            return Ok(new { message = result });  // Return the success message from LogOut method
        }



    }
}
