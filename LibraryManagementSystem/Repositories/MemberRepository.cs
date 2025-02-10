using LibraryManagementSystem.Data;
using LibraryManagementSystem.Dtos.Member;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Member> CreateMemberAsync(Member member)
    {
        await _context.Members.AddAsync(member);
        await _context.SaveChangesAsync();

        return member;
    }

    public async Task<List<Member>> GetAllMembersAsync()
    {
        return await _context.Members
            .Include(m => m.LibraryCard)
            .ToListAsync();
    }

    public async Task<Member?> GetMemberByIdAsync(int id)
    {
        var member = await _context.Members
            .Include(m => m.LibraryCard)
            .FirstOrDefaultAsync(m => m.Id == id);

        return member;
    }

    public async Task<Member?> UpdateMemberAsync(int id, UpdateMemberRequestDto requestMember)
    {
        var member = await _context.Members
            .Include(member => member.LibraryCard)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (member is null)
        {
            return null;
        }

        member.Name = requestMember.Name;
        member.LibraryCard.CardNumber = requestMember.CardNumber;

        await _context.SaveChangesAsync();

        return member;
    }

    public async Task<Member?> DeleteMemberAsync(int id)
    {
        var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);

        if (member is null)
        {
            return null;
        }

        _context.Members.Remove(member);
        await _context.SaveChangesAsync();

        return member;
    }
}