using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models.DTO
{
    public class SignInUserDto
    {
        [Required(ErrorMessage = "E-mail обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}
