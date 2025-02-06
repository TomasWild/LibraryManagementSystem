using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

[Table("Books")]
public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Synopsis { get; set; }

    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public ICollection<BookCategory> BookCategories { get; set; } = [];
}