using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // POST /api/user/register - User registration endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterUser(user);
            if (!result)
                return BadRequest("User already exists");

            return Ok("Registration successful");
        }

        // POST /api/user/login - User login endpoint to authenticate and get JWT token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Authenticate the user and generate a JWT token
            var token = await _userService.AuthenticateUser(loginRequest.Email, loginRequest.Password);
            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new { Token = token });
        }

        // POST /api/user/logout - User logout endpoint (optional for invalidating the token in a session)
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Implement token invalidation logic if needed (e.g., blacklist the token or remove from client-side)
            return Ok("Logout successful");
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
