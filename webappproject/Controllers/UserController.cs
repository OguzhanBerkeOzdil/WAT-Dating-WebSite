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
    [Route("[controller]")]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(UserService userService, BanService banService) : base(banService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("{ProfileUrl}")]
        public IActionResult Profile(string ProfileUrl)
        {
            var user = _userService.Get(x => x.ProfileUrl == ProfileUrl).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet("Matches")]
        public IActionResult Matches()
        {
            try
            {
                // Check if user is authenticated
                if (!User.Identity?.IsAuthenticated == true)
                {
                    TempData["Error"] = "Please log in to view matches.";
                    return RedirectToAction("Index", "Login");
                }

                var currentUserEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUserEmail))
                {
                    TempData["Error"] = "Authentication error. Please log in again.";
                    return RedirectToAction("Index", "Login");
                }

                var currentUser = _userService.Get(x => x.Email == currentUserEmail).FirstOrDefault();
                
                if (currentUser == null)
                {
                    TempData["Error"] = "User not found. Please log in again.";
                    return RedirectToAction("Index", "Login");
                }

                // Check if user is banned
                if (_banService.IsBanned(currentUserEmail))
                {
                    TempData["Error"] = "Your account has been suspended. Please contact administration.";
                    return RedirectToAction("Index", "Home");
                }

                // Generate some dummy matches for demo
                var dummyMatches = GenerateDummyUsers(6);
                
                return View(dummyMatches);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading matches.";
                return View(new List<User>());
            }
        }

        [HttpGet("RandomProfiles")]
        public async Task<IActionResult> RandomProfiles()
        {
            try
            {
                // Check if user is authenticated
                if (!User.Identity?.IsAuthenticated == true)
                {
                    TempData["Error"] = "Please log in to discover profiles.";
                    return RedirectToAction("Index", "Login");
                }

                var currentUserEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentUserEmail))
                {
                    TempData["Error"] = "Authentication error. Please log in again.";
                    return RedirectToAction("Index", "Login");
                }

                var currentUser = _userService.Get(x => x.Email == currentUserEmail).FirstOrDefault();
                
                if (currentUser == null)
                {
                    TempData["Error"] = "User not found. Please log in again.";
                    return RedirectToAction("Index", "Login");
                }

                // Check if user is banned
                if (_banService.IsBanned(currentUserEmail))
                {
                    TempData["Error"] = "Your account has been suspended. Please contact administration.";
                    return RedirectToAction("Index", "Home");
                }

                // Get real users excluding current user or generate dummy users if no real users
                var users = await _userService.GetAllUsersAsync();
                var otherUsers = users.Where(u => u.Email != currentUserEmail && u.IsActive).ToList();
                
                if (otherUsers.Count == 0)
                {
                    // Generate dummy users for demo
                    otherUsers = GenerateDummyUsers(10);
                }

                ViewBag.CurrentUser = currentUser;
                return View(otherUsers);
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while loading profiles.";
                return View(new List<User>());
            }
        }

        private List<User> GenerateDummyUsers(int count)
        {
            var dummyUsers = new List<User>();
            var names = new[] { "Emma", "Olivia", "Sophia", "Isabella", "Charlotte", "Amelia", "Mia", "Harper", "Evelyn", "Abigail" };
            var surnames = new[] { "Wilson", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
            var bios = new[] { 
                "Love traveling and exploring new cultures! 🌍",
                "Coffee enthusiast and book lover ☕📚", 
                "Adventure seeker and nature lover 🏔️",
                "Passionate about art and photography 📸",
                "Fitness enthusiast and yoga instructor 🧘‍♀️",
                "Foodie and cooking enthusiast 👩‍🍳",
                "Music lover and guitar player 🎸",
                "Dog lover and volunteer 🐕",
                "Science nerd and stargazer 🔬⭐",
                "Entrepreneur and tech enthusiast 💻"
            };

            var emojis = new[] { "😊", "🌟", "💃", "🎨", "🏃‍♀️", "📖", "🎵", "🌺", "✨", "🦋" };

            for (int i = 0; i < count; i++)
            {
                var random = new Random(i + DateTime.Now.Millisecond);
                dummyUsers.Add(new User
                {
                    Id = 1000 + i,
                    Name = names[i % names.Length],
                    Surname = surnames[i % surnames.Length],
                    Email = $"demo{i}@example.com",
                    Age = random.Next(22, 35),
                    Biography = bios[i % bios.Length],
                    Tag1 = "Music",
                    Tag2 = "Travel",
                    IsActive = true,
                    CreatedDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                    ImagePath = $"https://via.placeholder.com/400x600/667eea/ffffff?text={emojis[i % emojis.Length]}"
                });
            }

            return dummyUsers;
        }
    }
}