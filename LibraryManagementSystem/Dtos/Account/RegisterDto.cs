using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos.Account;

public class RegisterDto
{
    [Required]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public required string ConfirmedPassword { get; set; }
}