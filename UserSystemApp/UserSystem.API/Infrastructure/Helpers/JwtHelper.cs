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

        //public int? ValidateToken(string token)
        //{
        //    if (token == null)
        //        return null;

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_config.JwtToken.Key);
        //    try
        //    {
        //        tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
        //            ClockSkew = TimeSpan.Zero
        //        }, out SecurityToken validatedToken);

        //        var jwtToken = (JwtSecurityToken)validatedToken;
        //        var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

        //        // return user id from JWT token if validation successful
        //        return userId;
        //    }
        //    catch
        //    {
        //        // return null if validation fails
        //        return null;
        //    }
        //}
    }
}