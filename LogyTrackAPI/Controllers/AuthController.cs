using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LogyTrackAPI.Services;
using logyTrack.Models;
using LogyTrackAPI.Infrastructure.Data.Customers;

namespace LogyTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;

        public AuthController(IUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NAME) || string.IsNullOrWhiteSpace(request.PASS) || string.IsNullOrWhiteSpace(request.TYPE))
                    return BadRequest(new { success = false, message = "All fields required" });

                var existingUser = await _userRepository.GetByNameAsync(request.NAME);
                if (existingUser != null)
                    return Conflict(new { success = false, message = "Username already exists" });

                if (request.PASS.Length < 8)
                    return BadRequest(new { success = false, message = "Password must be 8+ characters" });

                var newUser = new User
                {
                    NAME = request.NAME.Trim(),
                    PASS = _authService.HashPassword(request.PASS),
                    TYPE = request.TYPE.Trim(),
                    CreatedDate = DateTime.Now
                };

                var id = await _userRepository.CreateAsync(newUser);
                return Ok(new { success = true, message = "Registration successful", data = new { id, name = newUser.NAME, type = newUser.TYPE } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.NAME) || string.IsNullOrWhiteSpace(request.PASS))
                    return BadRequest(new { success = false, message = "Username and password required" });

                var user = await _userRepository.GetByNameAsync(request.NAME);
                if (user == null || !_authService.VerifyPassword(request.PASS, user.PASS))
                    return Unauthorized(new { success = false, message = "Invalid credentials" });

                var token = _authService.GenerateToken(user);
                return Ok(new { success = true, message = "Login successful", data = new { id = user.ID, name = user.NAME, type = user.TYPE, token } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}