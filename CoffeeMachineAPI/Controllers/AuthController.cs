// Kasutame vajalikke teeke, et teha API päringud ja autentimine
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
using CoffeeMachineAPI.Services;
using Newtonsoft.Json;


namespace CoffeeMachineAPI.Controllers
{
    // Määrame API lõpp-punkti tee, milleks on "api/auth"
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Andmebaasi kontekst ja JWT konfidentsiaalsed seaded
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret;
        private readonly string _jwtAudience;
        private readonly string _jwtIssuer;
        private readonly LoginLogService _loginLogService;
        private readonly IConfiguration _configuration;



        // Konstruktor, kus loeme konfigureerimisfailist JWT seotud andmed
        public AuthController(ApplicationDbContext context, IConfiguration configuration, LoginLogService loginLogService)
        {
            _context = context;
            _jwtSecret = configuration["JwtToken:Secret"];   // JWT saladus
            _jwtAudience = configuration["JwtToken:Audience"]; // JWT sihtgrupp
            _jwtIssuer = configuration["JwtToken:Issuer"];  // JWT väljastaja
            _loginLogService = loginLogService;
            _configuration = configuration; 

        }

        // "register" lõpp-punkti API päring kasutaja registreerimiseks
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            // Kontrollime, et paroolid vastavad
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");  // Kui paroolid ei vasta, anname vea
            }

            // Kontrollime, kas e-mail on juba kasutusel
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Email is already in use.");  // Kui e-mail on juba olemas, anname vea
            }

            // Loome uue kasutaja ja salvestame selle andmebaasi
            var user = new User { Email = registerDto.Email };
            user.SetPassword(registerDto.Password);  // Parooli seadmine
            _context.Users.Add(user);
            await _context.SaveChangesAsync();  // Salvestame muutused andmebaasis

            // Kui kõik läks hästi, tagastame õnnestumise sõnumi
            return Ok(new { message = "Registration successful." });
        }
        
        

        // "login" lõpp-punkti API päring kasutaja sisse logimiseks
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();            
            // Otsime kasutajat andmebaasist e-posti järgi
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // Kui kasutajat ei leita, tagastame autentimise vea
            if (user == null)
            {
                string error = "User not found.";
                await _loginLogService.LogUserLoginAsync(SystemUserIds.UnknownUser, ipAddress, error, "password");
                return Unauthorized(error); // Kui kasutajat ei leita, tagastame vea
            }

            // Kontrollime parooli õigsust
            if (!user.CheckPassword(loginDto.Password))
            {
                string error = "Invalid password.";
                await _loginLogService.LogUserLoginAsync(user.Id, ipAddress, error, "password");
                return Unauthorized(error);  // Kui parool ei sobi, tagastame vea

            }
            
            // Логируем успешный вход
            await _loginLogService.LogUserLoginAsync(user.Id, ipAddress, "Succes", "password");
            // Kui kõik on korras, genereerime JWT tokeni
            string token;
            if (user.IsAdmin)
            {
                token = _configuration["Admin:token"];            }
            else
            {
                token = GenerateJwtToken(user);
            }

            // Tagastame kasutajale JWT tokeni
            return Ok(new { Token = token, UserId = user.Id, IsAdmin = user.IsAdmin, BonusBalance = user.BonusBalance });        }
        
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new { message = "Logged out successfully." });
        }
        // Meetod JWT tokeni genereerimiseks
        private string GenerateJwtToken(User user)
        {
            // Seame üles JWT väited (claims), mis sisaldavad kasutaja ID-d, e-posti ja rolli
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")  // Kui kasutaja on administraator, määrame tema rolliks "Admin"
            };

            // Krüpteerimisvõti
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Loome JWT tokeni, määrates sellele kehtivusaja ja muud vajalikud andmed
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,  // Määrame väljastaja
                audience: _jwtAudience,  // Määrame sihtgrupi
                claims: claims,  // Määrame väited (claims)
                expires: DateTime.Now.AddHours(1),  // Kehtivusaeg on 1 tund
                signingCredentials: creds);  // Krüpteerimisvõtmed

            // Tagastame tokeni stringina
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}