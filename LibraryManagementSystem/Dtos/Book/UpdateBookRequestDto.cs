using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Dtos.Book;

public class UpdateBookRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title must not exceed 100 characters.")]
    public required string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Synopsis must not exceed 1000 characters.")]
    public string? Synopsis { get; set; }

    public int AuthorId { get; set; }

    [MinLength(1, ErrorMessage = "At least one category must be added.")]
    public List<int> CategoryIds { get; set; } = [];
}