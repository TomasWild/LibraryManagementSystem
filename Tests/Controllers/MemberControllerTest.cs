using AutoMapper;
using LibraryManagementSystem.Controllers;
using LibraryManagementSystem.Dtos.Member;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories.Interfaces;
using LibraryManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers;

public class MemberControllerTest
{
    private readonly Mock<IMemberRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MemberController _controller;

    public MemberControllerTest()
    {
        _mockRepository = new Mock<IMemberRepository>();
        _mockMapper = new Mock<IMapper>();
        var mockService = new Mock<ICacheService>();
        _controller = new MemberController(_mockRepository.Object, _mockMapper.Object, mockService.Object);
    }

    [Fact]
    public async Task CreateMember_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var memberRequest = new CreateMemberRequestDto { Name = "Test Member", CardNumber = "0001" };
        var member = new Member { Id = 1, Name = "Test Member" };
        var memberDto = new MemberDto { Id = 1, Name = "Test Member" };

        _mockRepository.Setup(m => m.CreateMemberAsync(It.IsAny<Member>()))
            .ReturnsAsync(member);
        _mockMapper.Setup(m => m.Map<Member>(It.IsAny<CreateMemberRequestDto>()))
            .Returns(member);
        _mockMapper.Setup(m => m.Map<MemberDto>(It.IsAny<Member>()))
            .Returns(memberDto);

        // Act
        var result = await _controller.CreateMember(memberRequest);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        _mockRepository.Verify(r => r.CreateMemberAsync(member), Times.Once);
        Assert.Equal(nameof(MemberController.GetMemberById), actionResult.ActionName);
        Assert.Equal(memberDto, actionResult.Value);
    }

    [Fact]
    public async Task CreateMember_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Name", "Name field is required");

        var invalidMemberRequest = new CreateMemberRequestDto { Name = "", CardNumber = "0001" };

        // Act
        var result = await _controller.CreateMember(invalidMemberRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);

        var validationErrors = badRequestResult.Value as SerializableError;
        Assert.NotNull(validationErrors);
        Assert.Contains("Name", validationErrors.Keys);
    }

    [Fact]
    public async Task GetAllMembers_MembersExist_ReturnsOkWithMappedDtos()
    {
        // Arrange
        var members = new List<Member>
        {
            new() { Id = 1, Name = "Test Member 1" },
            new() { Id = 2, Name = "Test Member 2" }
        };

        _mockRepository.Setup(r => r.GetAllMembersAsync())
            .ReturnsAsync(members);
        _mockMapper.Setup(m => m.Map<List<MemberDto>>(It.IsAny<List<Member>>()))
            .Returns((List<Member> src) => src.Select(m => new MemberDto
                { Id = m.Id, Name = m.Name }).ToList());

        // Act
        var result = await _controller.GetAllMembers() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var returnedBooks = result.Value as List<MemberDto>;
        Assert.NotNull(returnedBooks);
        Assert.Equal(members.Count, returnedBooks.Count);
    }

    [Fact]
    public async Task GetAllMembers_NoMembersExist_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllMembersAsync())
            .ReturnsAsync([]);
        _mockMapper.Setup(m => m.Map<List<MemberDto>>(It.IsAny<List<Member>>()))
            .Returns([]);

        // Act
        var result = await _controller.GetAllMembers() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var returnedBooks = Assert.IsType<List<MemberDto>>(result.Value);
        Assert.Empty(returnedBooks);
    }

    [Fact]
    public async Task GetMemberById_WhenMemberExists_ReturnsMemberDto()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetMemberByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Member { Id = 1, Name = "Test Member" });
        _mockMapper.Setup(m => m.Map<MemberDto>(It.IsAny<Member>()))
            .Returns(new MemberDto { Id = 1, Name = "Test Member" });

        // Act
        var result = await _controller.GetMemberById(1);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnedMember = Assert.IsType<MemberDto>(actionResult.Value);
        _mockRepository.Verify(r => r.GetMemberByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.NotNull(returnedMember);
        Assert.Equal(1, returnedMember.Id);
        Assert.Equal(200, actionResult.StatusCode);
    }

    [Fact]
    public async Task GetMemberById_MemberNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetMemberByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Member?)null);

        // Act
        var result = await _controller.GetMemberById(1);

        // Assert
        _mockRepository.Verify(r => r.GetMemberByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateMember_ReturnsUpdatedMember_WhenMemberExists()
    {
        // Arrange
        var existingMember = new Member { Id = 1, Name = "Test Member" };
        var updateMemberRequest = new UpdateMemberRequestDto { Name = "Updated Test Member", CardNumber = "1001" };

        _mockRepository.Setup(m => m.UpdateMemberAsync(existingMember.Id, updateMemberRequest))
            .ReturnsAsync(new Member { Id = existingMember.Id, Name = existingMember.Name });
        _mockMapper.Setup(m => m.Map<MemberDto>(It.IsAny<Member>()))
            .Returns((Member src) => new MemberDto { Id = src.Id, Name = src.Name });

        // Act
        var result = await _controller.UpdateMember(existingMember.Id, updateMemberRequest);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var member = Assert.IsType<MemberDto>(actionResult.Value);
        Assert.Equal(existingMember.Name, member.Name);
    }

    [Fact]
    public async Task UpdateMember_NonExistentMember_ReturnsNotFound()
    {
        // Arrange
        var updateMemberRequest = new UpdateMemberRequestDto { Name = "Updated Test Member", CardNumber = "1001" };

        _mockRepository.Setup(r => r.UpdateMemberAsync(It.IsAny<int>(), It.IsAny<UpdateMemberRequestDto>()))
            .ReturnsAsync((Member?)null);

        // Act
        var result = await _controller.UpdateMember(999, updateMemberRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteMember_ValidId_ReturnsNoContent()
    {
        // Arrange
        var member = new Member { Id = 1, Name = "Test Member" };

        _mockRepository.Setup(r => r.DeleteMemberAsync(member.Id))
            .ReturnsAsync(member);

        // Act
        var result = await _controller.DeleteMemberAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteMemberAsync(member.Id), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteMember_InvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteMemberAsync(It.IsAny<int>()))
            .ReturnsAsync((Member?)null);

        // Act
        var result = await _controller.DeleteMemberAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteMemberAsync(1), Times.Once);
        Assert.IsType<NotFoundResult>(result);
    }
}