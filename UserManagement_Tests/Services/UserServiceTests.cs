using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement_Application.DTOs;
using UserManagement_Application.Entities;
using UserManagement_Application.Exceptions;
using UserManagement_Application.Interfaces;
using UserManagement_Application.Services;

namespace UserManagement_Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock = new();
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _sut = new UserService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetUserAsync_WithExistingId_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "0851234567",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetUserByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _sut.GetUserAsync(user.Id);

            // Assert
            Assert.Equal(user.Id, result.Id);
            Assert.Equal("John", result.FirstName);
        }
        [Fact]
        public async Task GetUserAsync_WithUnknownId_ThrowsNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.GetUserByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetUserAsync(id));
        }
        
        [Fact]
        public async Task CreateUserAsync_WithValidRequest_ReturnsCreatedUser()
        {
            // Arrange
            var request = new CreateUserRequest("John", "Doe", "john@example.com", "0851234567", new DateTime(1990, 1, 1));

            _repositoryMock
                .Setup(r => r.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repositoryMock
                .Setup(r => r.CreateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);

            // Act
            var result = await _sut.CreateUserAsync(request);

            // Assert
            Assert.Equal("John", result.FirstName);
            Assert.Equal("john@example.com", result.Email);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task CreateUserAsync_WithDuplicateEmail_ThrowsConflictException()
        {
            // Arrange
            var request = new CreateUserRequest("John", "Doe", "john@example.com", "0851234567", new DateTime(1990, 1, 1));

            _repositoryMock
                .Setup(r => r.EmailExistsAsync(request.Email, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _sut.CreateUserAsync(request));
        }

        [Fact]
        public async Task UpdateUserAsync_WithExistingId_UpdatesAndReturnsUser()
        {
            // Arrange
            var existing = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Grace",
                LastName = "Hopper",
                Email = "grace@example.com",
                PhoneNumber = "0851234567",
                DateOfBirth = new DateTime(1985, 5, 5),
                CreatedAt = DateTime.UtcNow
            };
            var request = new UpdateUserRequest("Grace", "Murray Hopper", "grace.hopper@example.com", "0851234567", new DateTime(1985, 5, 5));

            _repositoryMock
                .Setup(r => r.GetUserByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);
            _repositoryMock
                .Setup(r => r.EmailExistsAsync(request.Email, existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repositoryMock
                .Setup(r => r.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);

            // Act
            var result = await _sut.UpdateUserAsync(existing.Id, request);

            // Assert
            Assert.Equal("Murray Hopper", result.LastName);
            Assert.Equal("grace.hopper@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_WithUnknownId_ThrowsNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateUserRequest("Grace", "Hopper", "grace@example.com", "0851234567", new DateTime(1985, 5, 5));

            _repositoryMock
                .Setup(r => r.GetUserByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateUserAsync(id, request));
        }

        [Fact]
        public async Task UpdateUserAsync_WithDuplicateEmail_ThrowsConflictException()
        {
            // Arrange
            var existing = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Grace",
                LastName = "Hopper",
                Email = "grace@example.com",
                PhoneNumber = "0851234567",
                DateOfBirth = new DateTime(1985, 5, 5),
                CreatedAt = DateTime.UtcNow
            };
            var request = new UpdateUserRequest("Grace", "Hopper", "taken@example.com", "0851234567", new DateTime(1985, 5, 5));

            _repositoryMock
                .Setup(r => r.GetUserByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);
            _repositoryMock
                .Setup(r => r.EmailExistsAsync(request.Email, existing.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _sut.UpdateUserAsync(existing.Id, request));
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingId_CompletesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.DeleteUserAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await _sut.DeleteUserAsync(id);

            // Assert
            _repositoryMock.Verify(r => r.DeleteUserAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_WithUnknownId_ThrowsNotFoundException()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repositoryMock
                .Setup(r => r.DeleteUserAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteUserAsync(id));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
    {
        new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "0851234567", DateOfBirth = new DateTime(1990, 1, 1), CreatedAt = DateTime.UtcNow },
        new() { Id = Guid.NewGuid(), FirstName = "Grace", LastName = "Hopper", Email = "grace@example.com", PhoneNumber = "0851234568", DateOfBirth = new DateTime(1985, 5, 5), CreatedAt = DateTime.UtcNow }
    };

            _repositoryMock
                .Setup(r => r.GetAllUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _sut.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.FirstName == "John");
            Assert.Contains(result, u => u.FirstName == "Grace");
        }
    }
}
