using AutoMapper;
using LibraryManagementSystem.Dtos.Member;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Mappers;

public class MemberProfile : Profile
{
    public MemberProfile()
    {
        CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.LibraryCard, opt => opt.MapFrom(src => src.LibraryCard));

        CreateMap<CreateMemberRequestDto, Member>()
            .ForMember(dest => dest.LibraryCard,
                opt => opt.MapFrom(src => new LibraryCard { CardNumber = src.CardNumber }));

        CreateMap<UpdateMemberRequestDto, Member>()
            .ForMember(dest => dest.LibraryCard,
                opt => opt.MapFrom(src => new LibraryCard { CardNumber = src.CardNumber }));
    }
}