using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webappproject.Models;
using webappproject.Services;

namespace webappproject.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UserController : Controller
    {
        UserService _userService = new UserService();

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("{ProfileUrl}")]
        public async Task<IActionResult> ProfileAsync(string ProfileUrl)
        {
            var user = _userService.Get(x => x.ProfileUrl == ProfileUrl).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> RandomProfilesAsync(string ProfileUrl1, string ProfileUrl2)
        {

            var cookieName = CookieAuthenticationDefaults.AuthenticationScheme;
            var cookieValue = Request.Cookies[cookieName];

            var ticket = await HttpContext.AuthenticateAsync(cookieName);

            var email = ticket.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var User = _userService.Get(x => x.Email == email).FirstOrDefault();

            var age1 = User.SliderValue1;
            var age2 = User.SliderValue2;

            var random = new Random();

            var userNameSurname1 = default(string);
            var userNameSurname2 = default(string);

            var userImagePath1 = default(string);
            var userImagePath2 = default(string);

            var userBio1 = default(string);
            var userBio2 = default(string);

            var userTag1 = default(string);
            var userTag2 = default(string);

            if (User.Gender == "Male")
            {
                //var femaleUsers = _userService.GetAll().Where(user => user.Gender == "Female").ToList();

                var femaleUsers = _userService.GetAll().Where(user => user.Gender == "Female" && user.Age >= age1 && user.Age <= age2).ToList();


                if (femaleUsers.Count > 0)
                {
                    var index1 = random.Next(0, femaleUsers.Count);
                    femaleUsers.RemoveAt(index1);

                    if (femaleUsers.Count > 0)
                    {
                        var index2 = random.Next(0, femaleUsers.Count);

                        if (index1 < femaleUsers.Count && index2 < femaleUsers.Count)
                        {
                            userNameSurname1 = String.Concat(femaleUsers[index1].Name, " ", femaleUsers[index1].Surname);
                            userNameSurname2 = String.Concat(femaleUsers[index2].Name, " ", femaleUsers[index2].Surname);

                            userImagePath1 = femaleUsers[index1].ImagePath;
                            userImagePath2 = femaleUsers[index2].ImagePath;

                            userBio1 = femaleUsers[index1].Biography;
                            userBio2 = femaleUsers[index2].Biography;

                            userTag1 = String.Concat(femaleUsers[index1].Tag1, ", ", femaleUsers[index1].Tag2);
                            userTag2 = String.Concat(femaleUsers[index2].Tag1, ", ", femaleUsers[index2].Tag2);

                            ProfileUrl1 = femaleUsers[index1].ProfileUrl;
                            ProfileUrl2 = femaleUsers[index2].ProfileUrl;

                            if(femaleUsers[index1] == femaleUsers[index2])
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            // Handle the case when the generated indices are out of range
                            // Redirect back to the same page or return a specific view
                            return RedirectToAction("Index"); // Example: Redirect to the "Index" action of the same controller
                        }
                    }
                    else
                    {
                        // Handle the case when there are no remaining female users
                        // Redirect back to the same page or return a specific view
                        return RedirectToAction("Index"); // Example: Redirect to the "Index" action of the same controller
                    }
                }
                else
                {
                    // Handle the case when there are no female users available
                    // Redirect back to the same page or return a specific view
                    return RedirectToAction("Index"); // Example: Redirect to the "Index" action of the same controller
                }


            }
            else if (User.Gender == "Female")
            {
                var maleUsers = _userService.GetAll().Where(user => user.Gender == "Male").ToList();

                var index1 = random.Next(0, maleUsers.Count);
                maleUsers.RemoveAt(index1);

                var index2 = random.Next(0, maleUsers.Count);

                userNameSurname1 = String.Concat(maleUsers[index1].Name, " ", maleUsers[index1].Surname);
                userNameSurname2 = String.Concat(maleUsers[index2].Name, " ", maleUsers[index2].Surname);

                userImagePath1 = maleUsers[index1].ImagePath;
                userImagePath2 = maleUsers[index2].ImagePath;

                userBio1 = maleUsers[index1].Biography;
                userBio2 = maleUsers[index2].Biography;

                ProfileUrl1 = maleUsers[index1].ProfileUrl;
                ProfileUrl2 = maleUsers[index2].ProfileUrl;       
            }

            var profileLink1 = Url.Action("Profile", "User", new { ProfileUrl = ProfileUrl1 });
            var profileLink2 = Url.Action("Profile", "User", new { ProfileUrl = ProfileUrl2 });

            ViewBag.ProfileLink1 = profileLink1;
            ViewBag.ProfileLink2 = profileLink2;

            ViewBag.UserNameSurnameFirst = userNameSurname1;
            ViewBag.UserNameSurnameSecond = userNameSurname2;

            ViewBag.UserImagePathFirst = userImagePath1;
            ViewBag.UserImagePathSecond = userImagePath2;

            ViewBag.UserBiography1 = userBio1;
            ViewBag.UserBiography2 = userBio2;

            ViewBag.UserTag1 = userTag1;
            ViewBag.UserTag2 = userTag2;

            return View();
        }


    }
}