using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Repositories;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly Services.UserService _userService;
        private readonly UserRepository _userRepository;

        public AuthController(Services.UserService userService, AuthService authService, UserRepository userRepository)
        {
            _userService = userService;
            _authService = authService;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userService.Authenticate(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                return Conflict("Username or Email already exists.");
            }

            var user = new User()
            {
                Email = registerDto.Email,
                Password = Services.UserService.HashPassword(registerDto.Password),
                Username = registerDto.Username
            };

            await _userRepository.AddAsync(user);

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _userRepository.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
            });
        }
    }
}