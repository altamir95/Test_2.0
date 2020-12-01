using WebApplication3.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication3
{
    public class CustomPasswordValidator : IPasswordValidator<User>
    {
        public int RequiredMinLength { get; set; } // минимальная длина
        public int RequiredMaxLength { get; set; } // максимальная длина

        public CustomPasswordValidator(int minLength, int maxLength)
        {
            RequiredMinLength = minLength;
            RequiredMaxLength = maxLength;
        }

        public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (String.IsNullOrEmpty(password) || password.Length < RequiredMinLength || password.Length > RequiredMaxLength)
            {
                errors.Add(new IdentityError
                {
                    Description = $"Минимальная длина пароля равна {RequiredMinLength} а максимальная {RequiredMaxLength}"
                });
            }

            string pattern = @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z]{6,20}";
            Regex regex = new Regex(pattern);

            if (!Regex.IsMatch(password, pattern) || regex.Replace(password, "") != "")
            {
                errors.Add(new IdentityError
                {
                    Description = "Пароль должен состоять только из цифр и латинских букв а так же вмещать себя хотяьы одну букву с большим регистром и с малым и хотябы одну цифру"
                });
            }
            return Task.FromResult(errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }
    }
}
