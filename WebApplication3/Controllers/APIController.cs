using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class APIController : ControllerBase
    {
        ApplicationContext db;
        UserManager<User> _userManager;
        public APIController(ApplicationContext context, UserManager<User> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<ActionResult<User>> GetUserWords()
        {
            User user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return NotFound();
            List<DictionaryViewModel> userDictinory = JsonSerializer.Deserialize<List<DictionaryViewModel>>(user.Dictionary);

            return new ObjectResult(userDictinory);
        }
        [HttpPut]
        public async Task<ActionResult<string>> EditUserDictinory(DictionaryViewModel dictionary)
        {
            if (dictionary == null)
            {
                return BadRequest();
            }
            if (User.Identity.Name == null)
            {
                return Ok("Вы не зарегестрированы");
            }
            User user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Ok("Пользователь не найден");
            }
            if (user.Dictionary == null)
            {
                List<DictionaryViewModel> userDictinory1 = new List<DictionaryViewModel>();
                userDictinory1.Add(dictionary);
                user.Dictionary = JsonSerializer.Serialize(userDictinory1);
                var result2 = await _userManager.UpdateAsync(user);
                return Ok("Успешно");
            }
            List<DictionaryViewModel> userDictinory = JsonSerializer.Deserialize<List<DictionaryViewModel>>(user.Dictionary);
            userDictinory.Add(dictionary);


            user.Dictionary = JsonSerializer.Serialize(userDictinory);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Успешно");

            }
            else
            {
                return Ok("Не успешно");
            }
        }
        // DELETE api/users/5
        [HttpPut]
        public async Task<ActionResult<string>> DeleteWord(DictionaryViewModel dictionary)
        {
            if (dictionary == null)
            {
                return BadRequest();
            }
            if (User.Identity.Name == null)
            {
                return Ok("Вы не зарегестрированы");
            }
            User user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Ok("Пользователь не найден");
            }
            if (user.Dictionary == null)
            {
                return Ok("Элеинь для удаления отувствует");
            }
            List<DictionaryViewModel> userDictinory = JsonSerializer.Deserialize<List<DictionaryViewModel>>(user.Dictionary);
            //userDictinory.Remove(dictionary);
            userDictinory = userDictinory.Where(p => p.Arab != dictionary.Arab).ToList();

            user.Dictionary = JsonSerializer.Serialize(userDictinory);
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok("Успешно");

            }
            else
            {
                return Ok("Не успешно");
            }
        }
    }
}
