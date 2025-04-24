using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TaskManager.Core.Abstractions;
using TaskManager.Exceptions;
using TaskManager.Models;

namespace TaskManager.Core.Services
{
    public class AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        public async Task<IdentityResult> RegisterAsync(string email, string password, string username)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result;
        }

        public async Task<SignInResult> LogInAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
        }

        public async Task<int> GetUserIdAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);

            return user is null ? throw new UserNotFoundException("Пользователь не найден") : user.Id;
        }

        public Task LogOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
    }
}
