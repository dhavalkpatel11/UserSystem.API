using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Infrastructure.Helpers;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Infrastructure.Security;

namespace UserSystem.API.Infrastructure.Filters
{
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Method))]
    public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            // authorization
            var handler = new JwtSecurityTokenHandler();
            string authHeader = context.HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrWhiteSpace(authHeader)) throw new CustomAppException("UnauthorizedAccess");

            authHeader = authHeader.Replace("Bearer ", string.Empty);
            var token = handler.ReadToken(authHeader) as JwtSecurityToken;
            if (token == null || token.Claims == null) throw new CustomAppException("UnauthorizedAccess");

            int userId = 0;
            if (token.Claims.Any(a => a.Type == Constants.ClaimTypes.UserId))
            {
                var claim = token.Claims.FirstOrDefault(claim => claim.Type == Constants.ClaimTypes.UserId);
                userId = Convert.ToInt32(claim?.Value);
            }

            var userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService));
            var user = await userService.Get(userId);
            if (user == null || user.Id <= 0) throw new CustomAppException("UnauthorizedAccess");
        }
    }
}
