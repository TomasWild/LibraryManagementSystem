using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

[Table("LibraryCards")]
public class LibraryCard
{
    public int Id { get; set; }
    public required string CardNumber { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
}