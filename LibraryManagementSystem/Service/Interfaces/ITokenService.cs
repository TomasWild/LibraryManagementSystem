using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Service.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}