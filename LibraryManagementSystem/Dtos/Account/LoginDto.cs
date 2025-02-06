using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos.Account;

public class LoginDto
{
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}