using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserSystem.API.Infrastructure.Filters;
using UserSystem.API.Interfaces.Services.Security;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Models.Request.Users;
using UserSystem.API.Models.Response;
using UserSystem.API.Models.Response.Users;
namespace UserSystem.API.Controllers.Users
{
    [CustomAuthorize]
    [ApiController]
    [Route("api/web/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenReaderService _tokenReader;
        private readonly IMapper _mapper;
        public UsersController(IUserService userService, IMapper mapper, ITokenReaderService tokenReader)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenReader = tokenReader;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProfile()
        {
            return Ok(ApiResponse.SuccessResponse(_mapper.Map<UserViewModel>(await _userService.Get(_tokenReader.GetUserId()))));
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> RegisterProfile(AddUserRequest request)
        {
            await _userService.Add(request);
            return Ok(ApiResponse.SuccessMessage("Profile added successfully"));
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> UpdateProfile(UpdateUserRequest request)
        {
            await _userService.Update(_tokenReader.GetUserId(), request);
            return Ok(ApiResponse.SuccessMessage("Profile updated successfully"));
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> RemoveProfile()
        {
            await _userService.Delete(_tokenReader.GetUserId());
            return Ok(ApiResponse.SuccessMessage("Profile deleted successfully"));
        }
    }
}