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
    public class TokenReaderServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly TokenReaderService _tokenReaderService;

        public TokenReaderServiceTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _tokenReaderService = new TokenReaderService(_httpContextAccessorMock.Object);
        }

        [Fact]
        public void GetUserId_ShouldReturnUserId_WhenTokenIsValid()
        {
            // Arrange
            var userId = "123";
            var claimType = Constants.ClaimTypes.UserId;
            var token = new JwtSecurityToken(claims: new[] { new Claim(claimType, userId) });

            var authHeader = "Bearer test-token";
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = authHeader;

            _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(context);

            // Act
            var result = _tokenReaderService.GetUserId();

            // Assert
            Assert.Equal(int.Parse(userId), result);
        }

        [Fact]
        public void GetUserId_ShouldThrowException_WhenTokenIsMalformed()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = "Bearer malformed-token";

            _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(context);

            // Act & Assert
            Assert.Throws<FormatException>(() => _tokenReaderService.GetUserId());
        }

        [Fact]
        public void GetUserId_ShouldReturnZero_WhenAuthorizationHeaderIsMissing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(context);

            // Act
            var result = _tokenReaderService.GetUserId();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetUserId_ShouldHandleEmptyClaimsGracefully()
        {
            // Arrange
            var token = new JwtSecurityToken();
            var authHeader = "Bearer empty-token";

            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = authHeader;

            _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(context);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _tokenReaderService.GetUserId());
        }

        [Fact]
        public void GetTokenClaim_ShouldReturnClaimValue_WhenClaimExists()
        {
            // Arrange
            var claimType = Constants.ClaimTypes.UserId;
            var claimValue = "456";
            var token = new JwtSecurityToken(claims: new[] { new Claim(claimType, claimValue) });

            var authHeader = "Bearer token-with-claim";
            var context = new DefaultHttpContext();
            context.Request.Headers.Authorization = authHeader;

            _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(context);

            // Act
            var result = _tokenReaderService.GetUserId();

            // Assert
            Assert.Equal(int.Parse(claimValue), result);
        }
    }
}
