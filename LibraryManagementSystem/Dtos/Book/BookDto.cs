namespace LibraryManagementSystem.Dtos.Book;

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Synopsis { get; set; }
    public int AuthorId { get; set; }
    public required string AuthorName { get; set; }
    public List<string> Categories { get; set; } = [];
}