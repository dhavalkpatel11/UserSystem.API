using AutoMapper;
using UserSystem.API.Infrastructure.Helpers;
using UserSystem.API.Interfaces.Services.Security;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Models.Request.Security;
using UserSystem.API.Models.Response.Security;

namespace UserSystem.API.Services.Users
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IJwtHelper _jwtHelper;
        public SecurityService(IUserService userService, IMapper mapper, IJwtHelper jwtHelper)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
        }

        public async Task<LoginViewModel> Login(LoginRequest request)
        {
            var user = await _userService.Get(request.Email);

            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new CustomAppException("Username or password is incorrect");

            // authentication successful
            var response = _mapper.Map<LoginViewModel>(user);
            response.Token = _jwtHelper.GenerateToken(user);
            return response;
        }
    }
}