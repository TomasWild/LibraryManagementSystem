namespace LibraryManagementSystem.Utils;

public class QueryObject
{
    public string? Title { get; set; }
    public int? CategoryId { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}