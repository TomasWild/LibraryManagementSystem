using AutoMapper;
using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BookController(IBookRepository bookRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    [HttpPost]
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
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookRepository.GetAllBooksAsync();
        var booksDto = books.Select(_mapper.Map<BookDto>).ToList();

        return Ok(booksDto);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookById([FromRoute] int id)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        var bookDto = _mapper.Map<BookDto>(book);

        return Ok(bookDto);
    }

    [HttpPut("{id:int}")]
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