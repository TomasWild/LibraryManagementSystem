using AutoMapper;
using LibraryManagementSystem.Dtos.Member;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Librarian")]
public class MemberController : ControllerBase
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public MemberController(IMemberRepository memberRepository, IMapper mapper, ICacheService cacheService)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMember([FromBody] CreateMemberRequestDto memberRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = _mapper.Map<Member>(memberRequestDto);
        var createdMember = await _memberRepository.CreateMemberAsync(member);
        var memberDto = _mapper.Map<MemberDto>(createdMember);

        return CreatedAtAction(nameof(GetMemberById), new { id = createdMember.Id }, memberDto);
    }

    [HttpGet]
    public async Task<ActionResult<MemberDto>> GetAllMembers()
    {
        var cacheKey = $"members_{DateTime.UtcNow:yyyyMMdd}";
        var cachedMembers = await _cacheService.GetAsync<List<MemberDto>>(cacheKey);

        if (cachedMembers is not null)
        {
            Ok(cachedMembers);
        }

        var members = await _memberRepository.GetAllMembersAsync();
        var membersDto = members.Select(_mapper.Map<MemberDto>).ToList();

        await _cacheService.SetAsync(cacheKey, membersDto);

        return Ok(membersDto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetMemberById([FromRoute] int id)
    {
        var cacheKey = $"member_{id}";
        var cachedMember = await _cacheService.GetAsync<MemberDto>(cacheKey);

        if (cachedMember is not null)
        {
            return Ok(cachedMember);
        }

        var member = await _memberRepository.GetMemberByIdAsync(id);

        if (member is null)
        {
            return NotFound();
        }

        var memberDto = _mapper.Map<MemberDto>(member);

        await _cacheService.SetAsync(cacheKey, memberDto);

        return Ok(memberDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMember([FromRoute] int id,
        [FromBody] UpdateMemberRequestDto updateMemberRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await _memberRepository.UpdateMemberAsync(id, updateMemberRequest);

        if (member is null)
        {
            return NotFound();
        }

        var memberDto = _mapper.Map<MemberDto>(member);

        return Ok(memberDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.DeleteMemberAsync(id);

        if (member is null)
        {
            return NotFound();
        }

        return NoContent();
    }
}