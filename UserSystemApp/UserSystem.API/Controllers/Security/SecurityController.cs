using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UserSystem.API.Interfaces.Services.Security;
using UserSystem.API.Models.Request.Security;
using UserSystem.API.Models.Response;

namespace UserSystem.API.Controllers.Security
{
    [ApiController]
    [Route("api/web/security")]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMapper _mapper;
        public SecurityController(ISecurityService securityService, IMapper mapper)
        {
            _securityService = securityService;
            _mapper = mapper;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return Ok(ApiResponse.SuccessResponse(await _securityService.Login(request)));
        }
    }
}