using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AuthApp.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using AuthApp.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using kyrsiv.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private UserContext db;
        public AccountController(UserContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.User = User.Identity.Name;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация
                    ViewBag.User = User.Identity.Name;
                    return RedirectToAction("AdminPanel", "Account");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            ViewBag.User = User.Identity.Name;
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.User = User.Identity.Name;
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    db.Users.Add(new User { Email = model.Email, Password = model.Password });
                    await db.SaveChangesAsync();

                    ViewBag.User = User.Identity.Name;
                    return RedirectToAction("Login", "Account");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            ViewBag.User = User.Identity.Name;
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.User = User.Identity.Name;
            return RedirectToAction("Login", "Account");
        }
        [Authorize]
        public IActionResult AdminPanel()
        {
            ViewBag.User = User.Identity.Name;
            return View(db.Messengs);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Editing(int? id)
        {
            try
            {
                if (id != null)
                {
                    var messeng = await db.Messengs.FirstOrDefaultAsync(p => p.Id == id);
                    ViewBag.Messeng = messeng;
                    ViewBag.MessengId = id;
                    ViewBag.User = User.Identity.Name;
                    return View();
                }
                ViewBag.Error = "Ошибка";
                ViewBag.User = User.Identity.Name;
                return RedirectToAction("AdminPanel", "Account");
            }
            catch
            {
                ViewBag.Error = "Ошибка";
                ViewBag.User = User.Identity.Name;
                return RedirectToAction("AdminPanel", "Account");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Editing(Messeng messeng)
        {
            try
            {
                db.Messengs.Update(messeng);
                await db.SaveChangesAsync();
                ViewBag.User = User.Identity.Name;
                return RedirectToAction("AdminPanel", "Account");
            }
            catch
            {
                ViewBag.Error = "Ошибка";
                ViewBag.User = User.Identity.Name;
                return RedirectToAction("AdminPanel", "Account");
            }
        }
        [Authorize]
        [HttpGet]
        public IActionResult Moder()
        {
            if (User.Identity.Name == "Admin")
            {
                ViewBag.User = User.Identity.Name;
                return View(db.Users);
            }
            else
            {
                return RedirectToAction("AdminPanel", "Account");
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddModer(User user)
        {

            if (user != null && User.Identity.Name == "Admin")
            {  
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Moder");
            }
            ViewBag.Error = "Ошибка при добавлении модератора";
            return RedirectToAction("Moder");

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            try
            {
                if (id != null && User.Identity.Name == "Admin")
                {
                    User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Moder");
                    }
                }

            }
            catch
            {
                ViewBag.Error = "Ошибка при удалении модератора";
                return RedirectToAction("Moder");
            }
            ViewBag.Error = "Ошибка при удалении модератора";
            return RedirectToAction("Moder");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id != null)
                {
                    Messeng messeng = await db.Messengs.FirstOrDefaultAsync(p => p.Id == id);
                    if (messeng != null)
                    {
                        db.Messengs.Remove(messeng);
                        await db.SaveChangesAsync();
                        return RedirectToAction("AdminPanel");
                    }
                }

            }
            catch
            {
                ViewBag.Error = "Ошибка при удалении";
                return RedirectToAction("AdminPanel");
            }
            ViewBag.Error = "Ошибка при удалении";
            return RedirectToAction("AdminPanel");
        }
    }
    
}
