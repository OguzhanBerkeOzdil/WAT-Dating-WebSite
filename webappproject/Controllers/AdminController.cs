using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using webappproject.MVVM;
using webappproject.Services;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using webappproject.Models;

namespace webappproject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        UserService _userService = new UserService();

        BanService _banService = new BanService();

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Detail(string genderFilter)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            var userList = _userService.GetAll();

            if (!string.IsNullOrEmpty(genderFilter))
            {
                userList = userList.Where(u => u.Gender == genderFilter).ToList();
            }

            userList.RemoveAll(u => u.Email == "admin@gmail.com");

            ViewBag.GenderFilter = genderFilter; 

            return View(userList);
        }

        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            var user = _userService.GetById(userId);

            if (user != null)
            {
                var email = user.Email;
                var model = new BannedUser { Email = email };
                _banService.Add(model);

                _userService.Remove(user.Id);
                return RedirectToAction("Detail");
            }
            else
            {
                return RedirectToAction("Detail");
            }
        }

        [HttpGet]
        public async Task<ActionResult> BannedUserDetail()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            var userList = _banService.GetAll();

            return View(userList);
        }

        [HttpPost]
        public ActionResult UnbanUser(int emailId)
        {
            var bannedUser = _banService.GetById(emailId);

            if (bannedUser != null)
            {
                _banService.Remove(bannedUser.Id);
                return RedirectToAction("BannedUserDetail");
            }
            else
            {
                return RedirectToAction("BannedUserDetail");
            }
        }

    }
}

