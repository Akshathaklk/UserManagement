using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement_Application.DTOs;
using UserManagement_Application.Entities;
using UserManagement_Application.Exceptions;
using UserManagement_Application.Interfaces;

namespace UserManagement_Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"User with id '{id}' was not found.");

            return MapToResponse(user);
        }
        public async Task<IReadOnlyList<UserResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllUsersAsync(cancellationToken);

            return users.Select(MapToResponse).ToList();
        }
        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
            {
                throw new ConflictException($"A user with email '{request.Email}' already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateUserAsync(user, cancellationToken);

            return MapToResponse(created);
        }

        public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var existing = await _userRepository.GetUserByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"User with id '{id}' was not found.");

            if (await _userRepository.EmailExistsAsync(request.Email, id, cancellationToken))
            {
                throw new ConflictException($"A user with email '{request.Email}' already exists.");
            }

            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Email = request.Email;
            existing.PhoneNumber = request.PhoneNumber;
            existing.DateOfBirth = request.DateOfBirth;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _userRepository.UpdateUserAsync(existing, cancellationToken);

            return MapToResponse(updated);
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var deleted = await _userRepository.DeleteUserAsync(id, cancellationToken);

            if (!deleted)
            {
                throw new NotFoundException($"User with id '{id}' was not found.");
            }
        }

        private static UserResponse MapToResponse(User user) => new(
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.PhoneNumber,
        user.DateOfBirth,
        user.CreatedAt,
        user.UpdatedAt);
    }
}
