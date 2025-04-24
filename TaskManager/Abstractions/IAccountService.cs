using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManager.Models;

namespace TaskManager.Core.Abstractions
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(string email, string login, string password);
        Task<SignInResult> LogInAsync(string email, string password);
        Task LogOutAsync();
        Task<int> GetUserIdAsync(ClaimsPrincipal claims);
    }
}
