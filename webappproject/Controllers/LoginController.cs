using Microsoft.AspNetCore.Mvc;
using webappproject.Services;
using webappproject.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace webappproject.Controllers
{
    public class LoginController : Controller
    {
        UserService _userService = new UserService();

            //GET: Login
        public IActionResult Index()
        {
        return View();
        }

        [HttpPost]
        public async  Task<IActionResult> Login(User model)
        {
            if (_userService.IsAdmin(model.Email, model.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index","Admin");
            }
            else if (_userService.IsCustomer(model.Email, model.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "MyAccount");
            }
            else
            {
                TempData["LoginError"] = "Email or password is wrong";
                return RedirectToAction("Index", "Login");
            }

        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

    }
}
