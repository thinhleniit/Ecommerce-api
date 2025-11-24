using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceApi.DTO.Authen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            // DEMO CHECK (replace with DB lookup later)
            string role;
            if (dto.Email == "admin@test.com" && dto.Password == "123456")
            {
                role = "Admin";
            }
            else if (dto.Email == "staff@test.com" && dto.Password == "123456")
            {
                role = "Staff";
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }

            var token = GenerateJwtToken(dto.Email, role);

            return Ok(new 
            { 
                token,
                role 
            });
        }

        private string GenerateJwtToken(string email, string role)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new[]
            {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        [HttpGet("auth-check")]
        public IActionResult CheckAuth()
        {
            return Ok(new { message = "Your token is valid", user = User.Identity?.Name });
        }

    }
}

