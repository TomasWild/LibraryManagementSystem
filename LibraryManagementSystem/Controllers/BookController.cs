using System.Text.Json;
using AutoMapper;
using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Service.Interfaces;
using LibraryManagementSystem.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public BookController(IBookRepository bookRepository, IMapper mapper, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Librarian")]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequestDto bookRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = _mapper.Map<Book>(bookRequestDto);
        var createdBook = await _bookRepository.CreateBookAsync(book, bookRequestDto.CategoryIds);
        var bookDto = _mapper.Map<BookDto>(createdBook);

        return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, bookDto);
    }

    [HttpGet]
    [Authorize(Roles = "Admin, User, Librarian")]
    public async Task<IActionResult> GetAllBooks([FromQuery] QueryObject query)
    {
        var cacheKey = $"books_{JsonSerializer.Serialize(query)}";
        var cachedBooks = await _cacheService.GetAsync<List<BookDto>>(cacheKey);

        if (cachedBooks is not null)
        {
            return Ok(cachedBooks);
        }

        var books = await _bookRepository.GetAllBooksAsync(query);
        var booksDto = books.Select(_mapper.Map<BookDto>).ToList();

        await _cacheService.SetAsync(cacheKey, booksDto);

        return Ok(booksDto);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin, User, Librarian")]
    public async Task<IActionResult> GetBookById([FromRoute] int id)
    {
        var cacheKey = $"books_{id}";
        var cachedBook = await _cacheService.GetAsync<BookDto>(cacheKey);

        if (cachedBook is not null)
        {
            return Ok(cachedBook);
        }

        var book = await _bookRepository.GetBookByIdAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        var bookDto = _mapper.Map<BookDto>(book);

        await _cacheService.SetAsync(cacheKey, bookDto);

        return Ok(bookDto);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin, Librarian")]
    public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] UpdateBookRequestDto bookRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var book = await _bookRepository.UpdateBookByIdAsync(id, bookRequestDto);

        if (book is null)
        {
            return NotFound();
        }

        var bookDto = _mapper.Map<BookDto>(book);

        return Ok(bookDto);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook([FromRoute] int id)
    {
        var book = await _bookRepository.DeleteBookByIdAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return NoContent();
    }
}