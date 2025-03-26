using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CanteenWebApiLibrary.Dto;
using CanteenWebApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CanteenWebApiLibrary.Services
{
    public class AuthService : IAuthService
    {
        private readonly CanteenDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(CanteenDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> AuthenticateUser(LoginRequestDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
                return null;


            var roles = await GetUserRolesAsync(user.Id);


            return GenerateJwtToken(user, roles);
        }


        public async Task<List<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found.");


            var roles = new List<string> { user.Role.RoleName };
            return roles;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }


        private string GenerateJwtToken(User user, List<string> roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);


            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim("userId", user.Id.ToString()),
                    new Claim("role", user.Role.RoleName)
                };


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }






    }
}
