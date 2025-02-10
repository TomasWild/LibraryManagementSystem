using LibraryManagementSystem.Dtos.Member;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repositories.Interfaces;

public interface IMemberRepository
{
    Task<Member> CreateMemberAsync(Member member);
    Task<List<Member>> GetAllMembersAsync();
    Task<Member?> GetMemberByIdAsync(int id);
    Task<Member?> UpdateMemberAsync(int id, UpdateMemberRequestDto requestMember);
    Task<Member?> DeleteMemberAsync(int id);
}