using UserSystem.API.Models.Response.Security;
using LoginRequest = UserSystem.API.Models.Request.Security.LoginRequest;

namespace UserSystem.API.Interfaces.Services.Security
{
    public interface ISecurityService
    {
        Task<LoginViewModel> Login(LoginRequest request);
    }
}