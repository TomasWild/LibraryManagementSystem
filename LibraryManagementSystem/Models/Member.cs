using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

[Table("Members")]
public class Member
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public LibraryCard LibraryCard { get; set; } = null!;
}