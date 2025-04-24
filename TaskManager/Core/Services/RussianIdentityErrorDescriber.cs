using Microsoft.AspNetCore.Identity;

namespace TaskManager.Core.Services
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Пароль должен содержать хотя бы одну цифру."
            };
        }
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = "Пароль слишком короткий."
            };
        }
        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Пароль должен содержать хотя бы одну букву."
            };
        }
        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresLower),
                Description = "Пароль должен содержать хотя бы одну заглавную букву."
            };
        }
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Пароль должен содержать хотя бы один символ."
            };
        }
        public override IdentityError DuplicateUserName(string name)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Пользователь с именем {name} уже существует."
            };
        }
        public override IdentityError DuplicateEmail(string mail)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Пользователь с именем {mail} уже существует."
            };
        }
    }
}
