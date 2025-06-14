using back.Models;
using back.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace back.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class AuthenticateController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly TestContext _context;

        public AuthenticateController(TokenService tokenService, TestContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        // POST: api/user/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
                return Unauthorized("Identifiants invalides.");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Identifiants invalides.");

            var token = _tokenService.GenerateToken(user);
            return Ok(new { token });
        }

        // POST: api/user/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var hasher = new PasswordHasher<User>();
            User user = new User
            {
                Username = request.Username,
                Firstname = request.Firstname
            };
            user.Password = hasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("OK");
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
    }
}

