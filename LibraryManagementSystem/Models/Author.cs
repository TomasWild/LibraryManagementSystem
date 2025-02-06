using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

[Table("Authors")]
public class Author
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public ICollection<Book> Books { get; set; } = [];
}