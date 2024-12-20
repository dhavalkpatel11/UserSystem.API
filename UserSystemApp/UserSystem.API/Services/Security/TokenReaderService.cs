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
            int userId = 0;
            int.TryParse(GetTokenClaim(Constants.ClaimTypes.UserId), out userId);
            return userId;
        }

        private string? GetTokenClaim(string claimType)
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                authHeader = authHeader.Replace("Bearer ", string.Empty);
                if(!string.IsNullOrWhiteSpace(authHeader))
                {
                    var securityToken = handler.ReadToken(authHeader) as JwtSecurityToken;
                    return securityToken?.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
                }
            }
            return null;
        }
    }
}