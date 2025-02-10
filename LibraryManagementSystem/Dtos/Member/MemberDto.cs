using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Dtos.Member;

public class MemberDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public LibraryCard? LibraryCard { get; set; }
}