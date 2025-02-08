using AutoMapper;
using LibraryManagementSystem.Controllers;
using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers;

public class BookControllerTest
{
    [Fact]
    public async Task CreateBook_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        var bookRequest = new CreateBookRequestDto
            { Title = "Book Title", Synopsis = "Book synopsis.", CategoryIds = [1] };
        var book = new Book { Id = 1, Title = "Book Title", Synopsis = "Book synopsis." };
        var bookDto = new BookDto
            { Id = 1, Title = "Book Title", Synopsis = "Book synopsis.", AuthorName = "Author Name" };

        mockRepository.Setup(r => r.CreateBookAsync(It.IsAny<Book>(), It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(book);
        mockMapper.Setup(m => m.Map<Book>(It.IsAny<CreateBookRequestDto>()))
            .Returns(book);
        mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
            .Returns(bookDto);

        // Act
        var result = await controller.CreateBook(bookRequest);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        mockRepository.Verify(r => r.CreateBookAsync(book, bookRequest.CategoryIds), Times.Once);
        Assert.Equal(nameof(BookController.GetBookById), actionResult.ActionName);
        Assert.Equal(bookDto, actionResult.Value);
    }

    [Fact]
    public async Task CreateBook_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        controller.ModelState.AddModelError("Title", "The Title field is required.");

        var invalidBookRequest = new CreateBookRequestDto
        {
            Title = "",
            Synopsis = "Valid synopsis",
            CategoryIds = []
        };

        // Act
        var result = await controller.CreateBook(invalidBookRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);

        var validationErrors = badRequestResult.Value as SerializableError;
        Assert.NotNull(validationErrors);
        Assert.Contains("Title", validationErrors.Keys);
    }

    [Fact]
    public async Task GetAllBooks_BooksExist_ReturnsOkWithMappedDtos()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        var books = new List<Book>
        {
            new() { Id = 1, Title = "Book Title 1", Synopsis = "Book 1 synopsis." },
            new() { Id = 2, Title = "Book Title 2", Synopsis = "Book 2 synopsis." },
            new() { Id = 3, Title = "Book Title 3", Synopsis = "Book 3 synopsis." }
        };

        mockRepository.Setup(r => r.GetAllBooksAsync(It.IsAny<QueryObject>()))
            .ReturnsAsync(books);
        mockMapper.Setup(m => m.Map<List<BookDto>>(It.IsAny<List<Book>>()))
            .Returns((List<Book> src) => src.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Synopsis = b.Synopsis,
                AuthorName = $"{b.Author.FirstName} {b.Author.LastName}"
            }).ToList());

        // Act
        var result = await controller.GetAllBooks(It.IsAny<QueryObject>()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var returnedBooks = result.Value as List<BookDto>;
        Assert.NotNull(returnedBooks);
        Assert.Equal(books.Count, returnedBooks.Count);
    }

    [Fact]
    public async Task GetAllBooks_NoBooksExist_ReturnsEmptyList()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        mockRepository.Setup(r => r.GetAllBooksAsync(It.IsAny<QueryObject>()))
            .ReturnsAsync([]);
        mockMapper.Setup(m => m.Map<List<BookDto>>(It.IsAny<List<Book>>()))
            .Returns([]);

        // Act
        var result = await controller.GetAllBooks(It.IsAny<QueryObject>()) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var returnedBooks = Assert.IsType<List<BookDto>>(result.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task GetBookById_WhenTodoExists_ReturnsTodoItemDto()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        mockRepository.Setup(r => r.GetBookByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Book { Id = 1, Title = "Book Title 1", Synopsis = "Book 1 synopsis." });
        mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
            .Returns(new BookDto
                { Id = 1, Title = "Book Title", Synopsis = "Book synopsis.", AuthorName = "Author Name" });

        // Act
        var result = await controller.GetBookById(1);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<BookDto>(actionResult.Value);
        mockRepository.Verify(r => r.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.NotNull(returnedBook);
        Assert.Equal(1, returnedBook.Id);
        Assert.Equal(200, actionResult.StatusCode);
    }

    [Fact]
    public async Task GetBookById_BookNotFound_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        mockRepository.Setup(r => r.GetBookByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await controller.GetBookById(1);

        // Assert
        mockRepository.Verify(r => r.GetBookByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateBook_ReturnsUpdatedBook_WhenBookExists()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        var existingBook = new Book
        {
            Id = 1, Title = "Book Title", Synopsis = "Book synopsis.",
            AuthorId = 1,
            Author = new Author { FirstName = "Author First Name", LastName = "Author Last Name" }
        };
        var bookRequest = new UpdateBookRequestDto
            { Title = "Updated Book Title", Synopsis = "Updated book synopsis." };

        mockRepository.Setup(r => r.UpdateBookByIdAsync(existingBook.Id, bookRequest))
            .ReturnsAsync(new Book
            {
                Id = existingBook.Id, Title = existingBook.Title, Synopsis = existingBook.Synopsis,
                AuthorId = existingBook.AuthorId,
                Author = new Author { FirstName = "Author First Name", LastName = "Author Last Name" }
            });
        mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
            .Returns((Book src) => new BookDto
            {
                Id = src.Id, Title = src.Title, Synopsis = src.Synopsis,
                AuthorName = $"{src.Author.FirstName} {src.Author.LastName}",
            });

        // Act
        var result = await controller.UpdateBook(existingBook.Id, bookRequest);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var book = Assert.IsType<BookDto>(actionResult.Value);
        Assert.Equal(existingBook.Title, book.Title);
        Assert.Equal(existingBook.Synopsis, book.Synopsis);
    }

    [Fact]
    public async Task UpdateBook_NonExistentBook_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        var updateRequest = new UpdateBookRequestDto
        {
            Title = "Updated Title",
            Synopsis = "Updated Synopsis"
        };

        mockRepository.Setup(r => r.UpdateBookByIdAsync(It.IsAny<int>(), It.IsAny<UpdateBookRequestDto>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await controller.UpdateBook(999, updateRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteBook_ValidId_ReturnsNoContent()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        var book = new Book { Id = 1, Title = "Book Title", Synopsis = "Book synopsis." };

        mockRepository.Setup(r => r.DeleteBookByIdAsync(book.Id))
            .ReturnsAsync(book);

        // Act
        var result = await controller.DeleteBook(1);

        // Assert
        mockRepository.Verify(r => r.DeleteBookByIdAsync(book.Id), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBook_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockRepository = new Mock<IBookRepository>();
        var mockMapper = new Mock<IMapper>();
        var controller = new BookController(mockRepository.Object, mockMapper.Object);

        mockRepository.Setup(r => r.DeleteBookByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await controller.DeleteBook(1);

        // Assert
        mockRepository.Verify(r => r.DeleteBookByIdAsync(1), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }
}