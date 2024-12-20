using System.IdentityModel.Tokens.Jwt;
using UserSystem.API.Infrastructure.Helpers;
using UserSystem.API.Interfaces.Services.Security;

namespace UserSystem.API.Services.Users
{
    public class TokenReaderService : ITokenReaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenReaderService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var userId = GetTokenClaim(Constants.ClaimTypes.UserId);
            return int.Parse(userId);
        }

        private string GetTokenClaim(string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                authHeader = authHeader.Replace("Bearer ", string.Empty);
                var token = handler.ReadToken(authHeader) as JwtSecurityToken;
                var id = token.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
                return id;
            }
            return null;
        }
    }
}