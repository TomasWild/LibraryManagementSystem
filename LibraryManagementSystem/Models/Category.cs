using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

[Table("Categories")]
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<BookCategory> BookCategories { get; set; } = [];
}