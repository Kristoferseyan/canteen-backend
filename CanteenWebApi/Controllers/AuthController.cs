using CanteenWebApiLibrary.Dto;
using CanteenWebApiLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CanteenWebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            var token = await _authService.AuthenticateUser(loginDto);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var user = await _authService.GetUserByUsernameAsync(loginDto.Username);
            var roles = await _authService.GetUserRolesAsync(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("userId", user.Id.ToString()),  
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
                Console.WriteLine($"Assigned role: {role}"); 
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("oIL6F9U4d3LUzXDL8HC6TwHX/xhrwGFUp5UhQd1GXyc="));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "CanteenWebApi",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwtToken) });
        }
    }
}
