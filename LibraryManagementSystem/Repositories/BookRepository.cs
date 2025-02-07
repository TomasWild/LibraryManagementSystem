using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Utils;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Book> CreateBookAsync(Book book, List<int> categoryIds)
    {
        var categories = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .ToListAsync();

        foreach (var category in categories)
        {
            book.BookCategories.Add(new BookCategory { Book = book, Category = category });
        }

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<List<Book>> GetAllBooksAsync(QueryObject query)
    {
        var books = _context.Books
            .Include(b => b.Author)
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Title))
        {
            books = books.Where(b => b.Title.Contains(query.Title));
        }

        if (query.CategoryId.HasValue)
        {
            books = books.Where(b => b.BookCategories.Any(bc => bc.CategoryId == query.CategoryId.Value));
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            books = query.SortBy.ToLowerInvariant() switch
            {
                "title" => query.IsDescending
                    ? books.OrderByDescending(b => b.Title)
                    : books.OrderBy(b => b.Title),
                "author" => query.IsDescending
                    ? books.OrderByDescending(b => b.Author.FirstName + " " + b.Author.LastName)
                    : books.OrderBy(b => b.Author.FirstName + " " + b.Author.LastName),
                _ => books
            };
        }

        var skipNumber = (query.PageNumber - 1) * query.PageSize;

        return await books.Skip(skipNumber).Take(query.PageSize).ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        return book;
    }

    public async Task<Book?> UpdateBookByIdAsync(int id, UpdateBookRequestDto bookRequestDto)
    {
        var book = await _context.Books
            .Include(b => b.Author)
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return null;
        }

        book.Title = bookRequestDto.Title;
        book.Synopsis = bookRequestDto.Synopsis;

        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == bookRequestDto.AuthorId);
        if (author != null)
        {
            book.Author = author;
        }

        var currentCategoryIds = book.BookCategories.Select(bc => bc.Category.Id).ToList();
        var newCategoryIds = bookRequestDto.CategoryIds;

        var categoriesToRemove = book.BookCategories
            .Where(bc => !newCategoryIds.Contains(bc.Category.Id))
            .ToList();

        foreach (var category in categoriesToRemove)
        {
            _context.BookCategories.Remove(category);
        }

        var categoriesToAdd = await _context.Categories
            .Where(c => newCategoryIds.Contains(c.Id) && !currentCategoryIds.Contains(c.Id))
            .ToListAsync();

        foreach (var category in categoriesToAdd)
        {
            book.BookCategories.Add(new BookCategory { Book = book, Category = category });
        }

        await _context.SaveChangesAsync();

        return book;
    }

    public async Task<Book?> DeleteBookByIdAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);

        if (book is null)
        {
            return null;
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return book;
    }
}