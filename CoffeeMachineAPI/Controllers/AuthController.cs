using CoffeeMachineAPI.Data;
using CoffeeMachineAPI.DTOs;
using CoffeeMachineAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace CoffeeMachineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret;
        private readonly string _jwtAudience;
        private readonly string _jwtIssuer;



        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["JwtToken:Secret"];
            _jwtAudience = configuration["JwtToken:Audience"];
            _jwtIssuer = configuration["JwtToken:Issuer"];

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            var user = new User { Email = registerDto.Email };
            user.SetPassword(registerDto.Password);  
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            if (!user.CheckPassword(loginDto.Password))
            {
                return Unauthorized("Invalid password.");
            }

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}