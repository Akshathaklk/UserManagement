using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement_Application.Validations;

namespace UserManagement_Application.DTOs
{
    public record CreateUserRequest(
    [Required, MaxLength(100)] string FirstName,
    [Required, MaxLength(100)] string LastName,
    [Required, EmailAddress, MaxLength(256)] string Email,
    [Required, Phone, MaxLength(20)] string PhoneNumber,
    [NotFutureDate] DateTime DateOfBirth);
}
