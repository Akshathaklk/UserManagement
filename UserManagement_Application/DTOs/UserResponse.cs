using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement_Application.Entities;

namespace UserManagement_Application.DTOs
{
    public record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateTime DateOfBirth,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
}
