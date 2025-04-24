using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Abstractions;
using TaskManager.Models.DTO;

namespace TaskManager.Controllers
{
    public class AccountController(IAccountService accountService) : Controller
    {
        private readonly IAccountService _accountService = accountService;

        [HttpGet]
        public IActionResult Auth()
        {
            return View(new AuthModel
            {
                RegisterUser = new RegisterUserDto(),
                SignInUser = new SignInUserDto()
            }); ;
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind(Prefix = "RegisterUser")] RegisterUserDto dto)
        {

            var result = await _accountService.RegisterAsync(dto.Email, dto.Password, dto.UserName);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                dto.Email = string.Empty;
                dto.Password = string.Empty;
                dto.UserName = string.Empty;

                return View("Auth", new AuthModel
                {
                    RegisterUser = dto,
                    SignInUser = new SignInUserDto()
                });
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn([Bind(Prefix = "SignInUser")] SignInUserDto dto)
        {
            var result = await _accountService.LogInAsync(dto.Email, dto.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Логин или пароль ведены неверно");

                return View("Auth", new AuthModel
                {
                    RegisterUser = new RegisterUserDto(),
                    SignInUser = dto
                });
            }

            return RedirectToAction("index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
           await _accountService.LogOutAsync();

           return RedirectToAction("Auth", "Account");
        }
    }
}
