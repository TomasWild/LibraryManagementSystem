using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Utils;

namespace LibraryManagementSystem.Repositories.Interfaces;

public interface IBookRepository
{
    Task<Book> CreateBookAsync(Book book, List<int> categoryIds);
    Task<List<Book>> GetAllBooksAsync(QueryObject query);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> UpdateBookByIdAsync(int id, UpdateBookRequestDto bookRequestDto);
    Task<Book?> DeleteBookByIdAsync(int id);
}