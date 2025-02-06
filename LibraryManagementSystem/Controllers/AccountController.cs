using LibraryManagementSystem.Dtos.Account;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser is not null)
        {
            return BadRequest("Email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        var newAccount = new NewAccountDto
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            Token = token
        };

        return Ok(newAccount);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var passwordValid = await _signInManager.CheckPasswordSignInAsync(
            user, loginDto.Password, false);
        if (!passwordValid.Succeeded)
        {
            return Unauthorized("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        var newAccount = new NewAccountDto
        {
            UserName = user.UserName,
            Email = user.Email,
            Token = token
        };

        return Ok(newAccount);
    }
}