using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webappproject.Models;
using webappproject.Services;
using webappproject.MVVM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Builder.Extensions;

namespace webappproject.Controllers
{
    [Authorize(Roles = "User")]
    public class MyAccountController : Controller
    {

        UserService _userService = new UserService();


        public async Task<IActionResult> Index()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();
            var model = new User { Name = user.Name, Surname = user.Surname, Email = user.Email, Biography = user.Biography, Tag1 = user.Tag1, Tag2 = user.Tag2 };

            var ImagePath = user.ImagePath;

            ViewBag.ImagePathView = ImagePath;

            if (user.FirstLogIn == true)
            {
                return RedirectToAction("ChangeBioTags", "Myaccount");
            }

            return View(model);
        }

        public async Task<ActionResult> ChangePassword()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            ChangePasswordVM model = new ChangePasswordVM { };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordVM model)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            if (model.oldPassword == user.Password)
            {
                if (model.newPassword == user.Password)
                {
                    TempData["error"] += "Your new password can not be same as your old password!";
                    return View(model);
                }
                else if (model.newPassword != model.newPasswordConfirmation)
                {
                    TempData["error"] += "Your new password does not match during verify!";
                    return View(model);
                }
                else
                {
                    user.Password = model.newPassword;
                    _userService.Update(user, user.Id);
                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Index", "Login");
                }
            }
            else
            {
                TempData["error"] += "Your old password is not correct!";
                return View(model);
            }
        }

        public async Task<ActionResult> ChangeEmail()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            ChangeEmailVM model = new ChangeEmailVM { };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeEmail(ChangeEmailVM model)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            if (_userService.CheckEmail(model.newEmail))
            {
                TempData["error"] += "This e-mail is already being used ";
                return View(model);
            }

            if (model.oldEmail == user.Email)
            {
                if (model.Password == user.Password)
                {
                    user.Email = model.newEmail;
                    _userService.Update(user, user.Id);

                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    TempData["error"] += "Your password is wrong!";
                }
            }
            else
            {
                TempData["error"] += "Your current e-mail is wrong!";
            }
            return View(model);
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveAccount()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            RemoveAccountVM model = new RemoveAccountVM { };

            return View(model);
        }

        public async Task<IActionResult> RemoveAccount(RemoveAccountVM model)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            if (model.Verify == "YES I WANT TO DELETE THE ACCOUNT")
            {
                _userService.Remove(user.Id);
                //TempData["Info"] += "The account has been deleted";
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            else if (model.Verify != "YES I WANT TO DELETE THE ACCOUNT")
            {
                TempData["error"] += "The text is incorrect!";
            }

            return View(model);
        }


        public async Task<ActionResult> Upload()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile imageFile, IFormFile imageFileSecond)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();


            if (imageFile != null && imageFile.Length > 0)
            {
                // Save the uploaded file to a desired location (e.g., wwwroot/images)
                var imagePath = Path.Combine("wwwroot", "images", imageFile.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save the image path to the database
                user.ImagePath = "/images/" + imageFile.FileName;

                if(user.FirstLogIn == true)
                {
                    user.FirstLogIn = false;
                }           

                _userService.Update(user, user.Id);

                return RedirectToAction("Index", "Myaccount");
            }

            if (imageFileSecond != null && imageFileSecond.Length > 0)
            {
                // Save the uploaded file to a desired location (e.g., wwwroot/images)
                var imagePathSecond = Path.Combine("wwwroot", "images", imageFileSecond.FileName);
                using (var stream = new FileStream(imagePathSecond, FileMode.Create))
                {
                    await imageFileSecond.CopyToAsync(stream);
                }

                // Save the image path to the database
                user.ImagePath2 = "/images/" + imageFileSecond.FileName;

                if (string.IsNullOrEmpty(user.ImagePath))
                {
                    TempData["Error"] = "If you have not uploaded the first image, you can not upload the second. Upload your first image";
                    return RedirectToAction("Upload", "Myaccount");
                }

                _userService.Update(user, user.Id);

                return RedirectToAction("Upload", "Myaccount");
            }


            return View();
        }

        public async Task<ActionResult> ChangeBioTags()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            ChangeBioTagsVM model = new ChangeBioTagsVM { };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeBioTags(ChangeBioTagsVM model)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            if (model.Tag1 == model.Tag2)
            {
                TempData["error"] += "Tag1 and Tag2 can not be chosen as same!";
            }

            user.Tag1 = model.Tag1;
            user.Tag2 = model.Tag2;
            user.Biography = model.Biography;
            _userService.Update(user, user.Id);

            if (user.FirstLogIn == true)
            {
                return RedirectToAction("Upload", "Myaccount");
            }

            return RedirectToAction("Index", "Myaccount");
        }

        public async Task<ActionResult> AgeRange()
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            ChangeAgeRange model = new ChangeAgeRange { };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AgeRange(ChangeAgeRange model)
        {
            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.Get(x => x.Email == email).FirstOrDefault();

            user.SliderValue1 = model.SliderValue1;
            user.SliderValue2 = model.SliderValue2;

            _userService.Update(user, user.Id);

            return RedirectToAction("Index", "Myaccount");
        }


    }
}