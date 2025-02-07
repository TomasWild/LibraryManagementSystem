using AutoMapper;
using LibraryManagementSystem.Dtos.Book;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Mappers;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author.FirstName + " " + src.Author.LastName))
            .ForMember(dest => dest.Categories,
                opt => opt.MapFrom(src => src.BookCategories.Select(bc => bc.Category.Name).ToList()));

        CreateMap<CreateBookRequestDto, Book>();

        CreateMap<UpdateBookRequestDto, Book>();
    }
}