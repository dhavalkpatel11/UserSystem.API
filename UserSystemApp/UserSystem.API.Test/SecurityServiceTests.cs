using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using UserSystem.API.Infrastructure.Helpers;
using UserSystem.API.Interfaces.Services.Security;
using UserSystem.API.Interfaces.Services.Users;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Models.Request.Security;
using UserSystem.API.Models.Response.Security;
using UserSystem.API.Services.Users;
using Xunit;

namespace UserSystem.API.Test
{
    public class SecurityServiceTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IJwtHelper> _jwtHelperMock;
        private readonly SecurityService _securityService;

        public SecurityServiceTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _jwtHelperMock = new Mock<IJwtHelper>();

            _securityService = new SecurityService(_userServiceMock.Object, _mapperMock.Object, _jwtHelperMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnLoginViewModel_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "StrongPass123" };
            var user = new User { Id = 1, Email = loginRequest.Email, Password = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password) };
            var token = "test-jwt-token";

            _userServiceMock.Setup(service => service.Get(loginRequest.Email)).ReturnsAsync(user);
            _jwtHelperMock.Setup(helper => helper.GenerateToken(user)).Returns(token);
            _mapperMock.Setup(mapper => mapper.Map<LoginViewModel>(user)).Returns(new LoginViewModel { Email = user.Email });

            // Act
            var result = await _securityService.Login(loginRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "nonexistent@example.com", Password = "password" };
            _userServiceMock.Setup(service => service.Get(loginRequest.Email)).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _securityService.Login(loginRequest));
            Assert.Equal("Username or password is incorrect", exception.Message);
        }

        [Fact]
        public async Task Login_ShouldThrowException_WhenPasswordIsInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
            var user = new User { Id = 1, Email = loginRequest.Email, Password = BCrypt.Net.BCrypt.HashPassword("CorrectPassword") };

            _userServiceMock.Setup(service => service.Get(loginRequest.Email)).ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _securityService.Login(loginRequest));
            Assert.Equal("Username or password is incorrect", exception.Message);
        }

        [Fact]
        public async Task Login_ShouldIncludeToken_WhenAuthenticationSucceeds()
        {
            // Arrange
            var loginRequest = new LoginRequest { Email = "user@example.com", Password = "ValidPassword123" };
            var user = new User { Id = 1, Email = loginRequest.Email, Password = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password) };
            var token = "jwt-token-123";

            _userServiceMock.Setup(service => service.Get(loginRequest.Email)).ReturnsAsync(user);
            _jwtHelperMock.Setup(helper => helper.GenerateToken(user)).Returns(token);
            _mapperMock.Setup(mapper => mapper.Map<LoginViewModel>(user)).Returns(new LoginViewModel { Email = user.Email });

            // Act
            var result = await _securityService.Login(loginRequest);

            // Assert
            Assert.NotNull(result.Token);
            Assert.Equal(token, result.Token);
        }
    }

}
