using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.ViewModels;
using WebApplication3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email,Dictionary = "[ { \"Arab\": \"خَرَجَ\", \"Rus\": \"выходить \" }, { \"Arab\": \"فِي\", \"Rus\": \"на\" }, { \"Arab\": \"يَكْتُبُ\", \"Rus\": \"пишет\" }, { \"Arab\": \"أُسْتَاذٌ\", \"Rus\": \"учитель\" }, { \"Arab\": \"رَجُلٌ\", \"Rus\": \"мужчина\" }, { \"Arab\": \"إِمْرَاَةٌ\", \"Rus\": \"женщина\" }, { \"Arab\": \"بَابٌ\", \"Rus\": \"дверь\" }, { \"Arab\": \"دَفْتَرٌ\", \"Rus\": \"тетрадь\" }, { \"Arab\": \"هَاتِ\", \"Rus\": \"дай\" }, { \"Arab\": \"جَمِيلٌ\", \"Rus\": \"красивый\" }, { \"Arab\": \"جَدِيدٌ\", \"Rus\": \"новый\" }, { \"Arab\": \"قَدِيمٌ\", \"Rus\": \"старый\" }, { \"Arab\": \"كَبِيرٌ\", \"Rus\": \"большой\" }, { \"Arab\": \"صَغِيرٌ\", \"Rus\": \"маленький\" }, { \"Arab\": \"يَسْكُنُ\", \"Rus\": \"живет\" }, { \"Arab\": \"نَشِيطٌ\", \"Rus\": \"активный\" }, { \"Arab\": \"إِقْرَأْ\", \"Rus\": \"читай\" }, { \"Arab\": \"خُذْ\", \"Rus\": \"возьми\" } ]" };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // генерация токена для пользователя
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(
                    //    "ConfirmEmail",
                    //    "Account",
                    //    new { userId = user.Id, code = code },
                    //    protocol: HttpContext.Request.Scheme);
                    //EmailService emailService = new EmailService();
                    //await emailService.SendEmailAsync(model.Email, "Подтвердите регистрацию",
                    //    $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");

                    //return Content("Для завершения регистрации проверьте электронную почту и перейдите по ссылке, указанной в письме");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByNameAsync(model.Email);
                //if (user != null)
                //{
                //    // проверяем, подтвержден ли email
                //    if (!await _userManager.IsEmailConfirmedAsync(user))
                //    {
                //        ModelState.AddModelError(string.Empty, "Вы не подтвердили свой email");
                //        return View(model);
                //    }
                //}

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // пользователь с данным email может отсутствовать в бд
                    // тем не менее мы выводим стандартное сообщение, чтобы скрыть 
                    // наличие или отсутствие пользователя в бд
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(model.Email, "Reset Password",
                    $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByNameAsync(id);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return Redirect("~/Home/Index"); 
        }
    }
}
