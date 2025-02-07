namespace LibraryManagementSystem.Dtos.Book;

public class UpdateBookRequestDto
{
    public required string Title { get; set; }
    public string? Synopsis { get; set; }
    public int AuthorId { get; set; }
    public List<int> CategoryIds { get; set; } = [];
}