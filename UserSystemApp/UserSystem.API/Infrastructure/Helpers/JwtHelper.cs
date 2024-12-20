using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Helpers;

namespace UserSystem.API.Infrastructure.Helpers
{
    public interface IJwtHelper
    {
        public string GenerateToken(User user);
        //public int? ValidateToken(string token);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly AppSettings _config;

        public JwtHelper(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }

        public string GenerateToken(User user)
        {
            // generate token that is valid for 1 day
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.JwtToken.Key));
            var jwtClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(Constants.ClaimTypes.UserId, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config.JwtToken.Issuer,
                audience: _config.JwtToken.Audience,
                claims: jwtClaims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}