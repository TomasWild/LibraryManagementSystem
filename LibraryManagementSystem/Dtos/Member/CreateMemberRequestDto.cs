namespace LibraryManagementSystem.Dtos.Member;

public class CreateMemberRequestDto
{
    public required string Name { get; set; }
    public required string CardNumber { get; set; }
}