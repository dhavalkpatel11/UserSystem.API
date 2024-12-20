
using AutoMapper;
using Moq;
using UserSystem.API.Interfaces.Repositories.Users;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Helpers;
using UserSystem.API.Models.Request.Users;
using UserSystem.API.Services.Users;
using Xunit;

namespace UserSystem.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Email = "test1@example.com" },
                new User { Id = 2, Email = "test2@example.com" }
            };

            _userRepoMock.Setup(repo => repo.GetAll()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count());
            Assert.Equal(users, result);
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            _userRepoMock.Setup(repo => repo.Get(user.Id)).ReturnsAsync(user);

            // Act
            var result = await _userService.Get(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetById_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Get(999));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Id = 1, Email = email };

            _userRepoMock.Setup(repo => repo.Get(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.Get(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetByEmail_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.Get(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Get("nonexistent@example.com"));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task Add_ShouldAddUser_WhenValidRequest()
        {
            // Arrange
            var request = new AddUserRequest { Email = "test@example.com", Password = "StrongPass@123" };
            var user = new User { Id = 1, Email = request.Email };

            _userRepoMock.Setup(repo => repo.CheckIfEmailAlreadyExists(request.Email, null)).ReturnsAsync(false);
            _mapperMock.Setup(mapper => mapper.Map<User>(request)).Returns(user);
            _userRepoMock.Setup(repo => repo.AddOrUpdateUser(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _userService.Add(request);

            // Assert
            _userRepoMock.Verify(repo => repo.AddOrUpdateUser(It.Is<User>(u => u.Email == request.Email)), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var request = new AddUserRequest { Email = "test@example.com", Password = "StrongPass@123" };
            _userRepoMock.Setup(repo => repo.CheckIfEmailAlreadyExists(request.Email, null)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Add(request));
            Assert.Equal($"This email '{request.Email}' is already taken.", exception.Message);
        }

        [Fact]
        public async Task Add_ShouldThrowException_WhenPasswordIsWeak()
        {
            // Arrange
            var request = new AddUserRequest { Email = "test@example.com", Password = "weak" };
            _userRepoMock.Setup(repo => repo.CheckIfEmailAlreadyExists(request.Email, null)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Add(request));
            Assert.StartsWith("Please enter strong password", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldUpdateUser_WhenValidRequest()
        {
            // Arrange
            var user = new User { Id = 1, Email = "old@example.com", Password = "hashed" };
            var request = new UpdateUserRequest { Email = "new@example.com", Password = "NewPass@123" };

            _userRepoMock.Setup(repo => repo.Get(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(repo => repo.CheckIfEmailAlreadyExists(request.Email, user.Id)).ReturnsAsync(false);
            _mapperMock.Setup(mapper => mapper.Map(request, user));
            _userRepoMock.Setup(repo => repo.AddOrUpdateUser(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            await _userService.Update(user.Id, request);

            // Assert
            _userRepoMock.Verify(repo => repo.AddOrUpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var user = new User { Id = 1, Email = "existing@example.com" };
            var request = new UpdateUserRequest { Email = "duplicate@example.com", Password = "NewPass@123" };

            _userRepoMock.Setup(repo => repo.Get(user.Id)).ReturnsAsync(user);
            _userRepoMock.Setup(repo => repo.CheckIfEmailAlreadyExists(request.Email, user.Id)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Update(user.Id, request));
            Assert.Equal($"This email '{request.Email}' is already taken.", exception.Message);
        }
        [Fact]
        public async Task Update_ShouldPreservePassword_WhenPasswordNotProvided()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com", Password = "hashed" };
            var request = new UpdateUserRequest { Email = "new@example.com" };

            _userRepoMock.Setup(repo => repo.Get(user.Id)).ReturnsAsync(user);
            _mapperMock.Setup(mapper => mapper.Map(request, user));

            // Act
            await _userService.Update(user.Id, request);

            // Assert
            Assert.Equal("hashed", user.Password);
            _userRepoMock.Verify(repo => repo.AddOrUpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldMarkUserAsDeleted()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            _userRepoMock.Setup(repo => repo.Get(user.Id)).ReturnsAsync(user);

            // Act
            await _userService.Delete(user.Id);

            // Assert
            Assert.NotNull(user.DeletedDate);
            _userRepoMock.Verify(repo => repo.AddOrUpdateUser(It.Is<User>(u => u.DeletedDate != null)), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepoMock.Setup(repo => repo.Get(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CustomAppException>(() => _userService.Delete(999));
            Assert.Equal("User not found", exception.Message);
        }
    }
}
