using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webappproject.Models;
using webappproject.Services;

namespace webappproject.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly UserService _userService;
        private readonly BanService _banService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserService userService, BanService banService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _banService = banService;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Get dashboard statistics
                var allUsers = await _userService.GetAllUsersAsync();
                var bannedUsers = await _banService.GetAllBannedUsersAsync();

                ViewBag.TotalUsers = allUsers.Count;
                ViewBag.ActiveUsers = allUsers.Count(u => u.IsActive);
                ViewBag.NewUsers = allUsers.Count(u => u.CreatedDate.Date == DateTime.Today);
                ViewBag.BannedUsers = bannedUsers.Count;

                // Get recent users for dashboard
                var recentUsers = allUsers
                    .OrderByDescending(u => u.CreatedDate)
                    .Take(5)
                    .ToList();
                ViewBag.RecentUsers = recentUsers;

                _logger.LogInformation("Admin dashboard accessed by {Email}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View();
            }
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Users(string search = "", string roleFilter = "", string statusFilter = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var bannedUsers = await _banService.GetAllBannedUsersAsync();
                var bannedUserIds = bannedUsers.Select(b => b.UserId).ToList();

                // Apply search filter
                if (!string.IsNullOrEmpty(search))
                {
                    _logger.LogInformation("Applying search filter: {Search}", search);
                    var beforeSearchCount = users.Count;
                    
                    users = users.Where(u => 
                        (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Surname) && u.Surname.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Name) && !string.IsNullOrEmpty(u.Surname) && 
                         (u.Name + " " + u.Surname).Contains(search, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                        
                    _logger.LogInformation("Search filter applied. Before: {BeforeCount}, After: {AfterCount} users", beforeSearchCount, users.Count);
                }

                // Apply role filter
                if (!string.IsNullOrEmpty(roleFilter))
                {
                    users = users.Where(u => u.Rol?.Name == roleFilter).ToList();
                }

                // Apply status filter
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    _logger.LogInformation("Applying status filter: {StatusFilter}. Total users before filter: {Count}", statusFilter, users.Count);
                    
                    // Log user states for debugging
                    var activeCount = users.Count(u => u.IsActive && !bannedUserIds.Contains(u.Id));
                    var inactiveCount = users.Count(u => !u.IsActive && !bannedUserIds.Contains(u.Id));
                    var bannedCount = users.Count(u => bannedUserIds.Contains(u.Id));
                    _logger.LogInformation("User breakdown - Active: {ActiveCount}, Inactive: {InactiveCount}, Banned: {BannedCount}", 
                        activeCount, inactiveCount, bannedCount);
                    
                    // Log the actual filter value received - use invariant culture for string comparison
                    var normalizedFilter = statusFilter.ToLowerInvariant();
                    _logger.LogInformation("Received status filter value: '{StatusFilter}', normalized: '{NormalizedFilter}'", 
                        statusFilter, normalizedFilter);
                    
                    var originalUserCount = users.Count;
                    switch (normalizedFilter)
                    {
                        case "active":
                            users = users.Where(u => u.IsActive && !bannedUserIds.Contains(u.Id)).ToList();
                            _logger.LogInformation("Active filter applied, filtered from {OriginalCount} to {Count} users", originalUserCount, users.Count);
                            break;
                        case "inactive":
                            users = users.Where(u => !u.IsActive && !bannedUserIds.Contains(u.Id)).ToList();
                            _logger.LogInformation("Inactive filter applied, filtered from {OriginalCount} to {Count} users", originalUserCount, users.Count);
                            break;
                        case "banned":
                            users = users.Where(u => bannedUserIds.Contains(u.Id)).ToList();
                            _logger.LogInformation("Banned filter applied, filtered from {OriginalCount} to {Count} users", originalUserCount, users.Count);
                            break;
                        default:
                            _logger.LogWarning("Unknown status filter value: '{StatusFilter}'. No filter applied.", statusFilter);
                            break;
                    }
                    
                    _logger.LogInformation("Status filter '{StatusFilter}' processing complete. Final user count: {Count}", statusFilter, users.Count);
                }

                // Pagination
                var totalUsers = users.Count;
                var paginatedUsers = users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.Search = search;
                ViewBag.RoleFilter = roleFilter;
                ViewBag.StatusFilter = statusFilter;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
                ViewBag.TotalUsers = totalUsers;
                ViewBag.BannedUsers = bannedUserIds;

                _logger.LogInformation("User management accessed by {Email}, showing {Count} users", User.Identity?.Name, paginatedUsers.Count);
                return View(paginatedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users list");
                TempData["Error"] = "An error occurred while loading users.";
                return View(new List<User>());
            }
        }

        [HttpGet("Users/{id}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Users");
                }

                _logger.LogInformation("User details viewed for {UserId} by admin {AdminEmail}", id, User.Identity?.Name);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user details for {UserId}", id);
                TempData["Error"] = "An error occurred while loading user details.";
                return RedirectToAction("Users");
            }
        }

        [HttpPost("BanUser/{id}")]
        public async Task<IActionResult> BanUser(int id, [FromBody] BanUserRequest request)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Don't allow banning other admins
                if (user.Rol?.Name == "Admin")
                {
                    return Json(new { success = false, message = "Cannot ban administrator accounts." });
                }

                user.IsActive = false;
                await _userService.UpdateUserAsync(user);

                // Add to banned users list
                var bannedUser = new BannedUser
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Reason = request.Reason ?? "Banned by administrator",
                    BanDate = DateTime.Now,
                    BannedBy = User.Identity?.Name ?? "System"
                };
                await _banService.BanUserAsync(bannedUser);
                
                _logger.LogInformation("User {UserId} ({Email}) banned by admin {AdminEmail}. Reason: {Reason}", 
                    id, user.Email, User.Identity?.Name, bannedUser.Reason);
                
                return Json(new { success = true, message = "User banned successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error banning user {UserId}", id);
                return Json(new { success = false, message = "An error occurred while banning the user." });
            }
        }

        [HttpPost("ActivateUser/{id}")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.IsActive = true;
                await _userService.UpdateUserAsync(user);

                // Remove from banned users list if exists
                var bannedUser = await _banService.GetBannedUserByUserIdAsync(user.Id);
                if (bannedUser != null)
                {
                    await _banService.UnbanUserAsync(bannedUser.Id);
                }
                
                _logger.LogInformation("User {UserId} ({Email}) activated by admin {AdminEmail}", 
                    id, user.Email, User.Identity?.Name);
                
                return Json(new { success = true, message = "User activated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user {UserId}", id);
                return Json(new { success = false, message = "An error occurred while activating the user." });
            }
        }

        [HttpGet("BannedUsers")]
        public async Task<IActionResult> BannedUsers()
        {
            try
            {
                var bannedUsers = await _banService.GetAllBannedUsersAsync();
                return View(bannedUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading banned users");
                TempData["Error"] = "An error occurred while loading banned users.";
                return View(new List<BannedUser>());
            }
        }

        [HttpPost("ExportData")]
        public async Task<IActionResult> ExportData()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var csv = "Id,Name,Surname,Email,Age,Gender,CreatedDate,IsActive\n";
                
                foreach (var user in users)
                {
                    csv += $"{user.Id},{user.Name},{user.Surname},{user.Email},{user.Age},{user.Gender},{user.CreatedDate:yyyy-MM-dd},{user.IsActive}\n";
                }
                
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
                return File(bytes, "text/csv", $"users_export_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data");
                return Json(new { success = false, message = "Export failed" });
            }
        }

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            try
            {
                // Simulate sending notification with await
                await Task.Delay(100); // Simulate async operation
                _logger.LogInformation("Notification sent: {Message} to {Target}", request.Message, request.Target);
                return Json(new { success = true, message = "Notification sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification");
                return Json(new { success = false, message = "Failed to send notification" });
            }
        }

        [HttpGet("EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Users");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user for edit {UserId}", id);
                TempData["Error"] = "An error occurred while loading user details.";
                return RedirectToAction("Users");
            }
        }

        [HttpPost("EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id, User model)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Users");
                }

                _logger.LogInformation("Updating user {UserId}. SliderValue1: {Slider1}, SliderValue2: {Slider2}", 
                    id, model.SliderValue1, model.SliderValue2);

                user.Name = model.Name;
                user.Surname = model.Surname;
                user.Email = model.Email;
                user.Age = model.Age;
                user.Gender = model.Gender;
                user.Biography = model.Biography;
                user.Tag1 = model.Tag1;
                user.Tag2 = model.Tag2;
                user.SliderValue1 = model.SliderValue1;
                user.SliderValue2 = model.SliderValue2;
                user.IsActive = model.IsActive;

                _logger.LogInformation("About to save user {UserId}. Final SliderValue1: {Slider1}, SliderValue2: {Slider2}", 
                    id, user.SliderValue1, user.SliderValue2);

                await _userService.UpdateUserAsync(user);
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction("Users");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                TempData["Error"] = "An error occurred while updating the user.";
                return RedirectToAction("Users");
            }
        }

        [HttpPost("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                await _userService.DeleteUserAsync(id);
                _logger.LogInformation("User {UserId} ({Email}) deleted by admin {AdminEmail}", 
                    id, user.Email, User.Identity?.Name);
                
                return Json(new { success = true, message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return Json(new { success = false, message = "An error occurred while deleting the user." });
            }
        }

        [HttpPost("ToggleUserStatus")]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                // Don't allow deactivating other admins
                if (user.Rol?.Name == "Admin" && user.IsActive)
                {
                    return Json(new { success = false, message = "Cannot deactivate administrator accounts." });
                }

                user.IsActive = !user.IsActive;
                await _userService.UpdateUserAsync(user);
                
                _logger.LogInformation("User {UserId} ({Email}) status toggled to {Status} by admin {AdminEmail}", 
                    userId, user.Email, user.IsActive ? "Active" : "Inactive", User.Identity?.Name);
                
                return Json(new { success = true, message = $"User {(user.IsActive ? "activated" : "deactivated")} successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling user status for {UserId}", userId);
                return Json(new { success = false, message = "An error occurred while updating user status." });
            }
        }

        [HttpPost("UnbanUser")]
        public async Task<IActionResult> UnbanUser([FromBody] UnbanUserRequest request)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(request.UserId);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found." });
                }

                user.IsActive = true;
                await _userService.UpdateUserAsync(user);

                // Remove from banned users list if exists
                var bannedUser = await _banService.GetBannedUserByUserIdAsync(user.Id);
                if (bannedUser != null)
                {
                    await _banService.UnbanUserAsync(bannedUser.Id);
                }
                
                _logger.LogInformation("User {UserId} ({Email}) unbanned by admin {AdminEmail}", 
                    request.UserId, user.Email, User.Identity?.Name);
                
                return Json(new { success = true, message = "User unbanned successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unbanning user {UserId}", request.UserId);
                return Json(new { success = false, message = "An error occurred while unbanning the user." });
            }
        }

        [HttpGet("Profile")]
        public IActionResult Profile()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("Admin profile access attempted without valid email claim");
                    return RedirectToAction("Logout", "Login");
                }

                var admin = _userService.Get(u => u.Email == email).FirstOrDefault();
                if (admin == null)
                {
                    _logger.LogWarning("Admin profile not found for email: {Email}", email);
                    return RedirectToAction("Logout", "Login");
                }

                _logger.LogInformation("Admin profile accessed by {Email}", email);
                return View(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing admin profile");
                TempData["ErrorMessage"] = "An error occurred while loading your profile.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("Profile")]
        public async Task<IActionResult> Profile(User model)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return RedirectToAction("Logout", "Login");
                }

                var admin = _userService.Get(u => u.Email == email).FirstOrDefault();
                if (admin == null)
                {
                    return RedirectToAction("Logout", "Login");
                }

                // Update only allowed fields for admin profile
                admin.Name = model.Name;
                admin.Surname = model.Surname;
                admin.Biography = model.Biography;
                admin.Age = model.Age;

                var success = await _userService.UpdateUserAsync(admin);
                if (success)
                {
                    TempData["SuccessMessage"] = "Admin profile updated successfully.";
                    _logger.LogInformation("Admin profile updated for {Email}", email);
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile.";
                    _logger.LogWarning("Failed to update admin profile for {Email}", email);
                }

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin profile");
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                return RedirectToAction("Profile");
            }
        }

        [HttpPost("ClearTestUserImages")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearTestUserImages()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var testUsers = users.Where(u => 
                    u.Name == "Emma Wilson" || 
                    u.Name == "de de" || 
                    u.Name == "ded ded" ||
                    (u.Email?.StartsWith("demo") == true) ||
                    (u.Email?.Contains("test") == true) ||
                    (u.Email?.Contains("example") == true)).ToList();

                int clearedCount = 0;
                var userDetails = new List<string>();

                foreach (var user in testUsers)
                {
                    bool hadImages = !string.IsNullOrEmpty(user.ImagePath) || !string.IsNullOrEmpty(user.ImagePath2);
                    
                    user.ImagePath = "";
                    user.ImagePath2 = "";
                    await _userService.UpdateUserAsync(user);
                    
                    if (hadImages)
                    {
                        clearedCount++;
                        userDetails.Add($"{user.Name} ({user.Email})");
                    }
                }

                _logger.LogInformation("Cleared images for {Count} test users. Users: {Users}", 
                    clearedCount, string.Join(", ", userDetails));
                
                var message = clearedCount > 0 
                    ? $"Successfully cleared images from {clearedCount} users:\n• " + string.Join("\n• ", userDetails.Take(5))
                    : $"Found {testUsers.Count} test users but none had images to clear.";
                
                TempData["Success"] = message;
                
                return Json(new { 
                    success = true, 
                    message = message,
                    totalTestUsers = testUsers.Count,
                    clearedCount = clearedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing test user images");
                return Json(new { success = false, message = "Error clearing images: " + ex.Message });
            }
        }

        [HttpPost("CheckTestUsers")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckTestUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var testUsers = users.Where(u => 
                    u.Name == "Emma Wilson" || 
                    u.Name == "de de" || 
                    u.Name == "ded ded" ||
                    (u.Email?.StartsWith("demo") == true) ||
                    (u.Email?.Contains("test") == true) ||
                    (u.Email?.Contains("example") == true)).ToList();

                var usersWithImages = 0;
                var usersWithoutImages = 0;
                var userDetails = new List<object>();

                foreach (var user in testUsers)
                {
                    bool hasImages = !string.IsNullOrEmpty(user.ImagePath) || !string.IsNullOrEmpty(user.ImagePath2);
                    
                    if (hasImages)
                    {
                        usersWithImages++;
                        var imagePaths = new List<string>();
                        if (!string.IsNullOrEmpty(user.ImagePath)) imagePaths.Add("Image1: " + user.ImagePath);
                        if (!string.IsNullOrEmpty(user.ImagePath2)) imagePaths.Add("Image2: " + user.ImagePath2);
                        
                        userDetails.Add(new { 
                            name = user.Name + " " + user.Surname, 
                            email = user.Email, 
                            hasImages = true,
                            imagePaths = string.Join(", ", imagePaths)
                        });
                    }
                    else
                    {
                        usersWithoutImages++;
                        userDetails.Add(new { 
                            name = user.Name + " " + user.Surname, 
                            email = user.Email, 
                            hasImages = false,
                            imagePaths = "No images"
                        });
                    }
                }

                _logger.LogInformation("Test user check: {Total} total, {WithImages} with images, {WithoutImages} without images", 
                    testUsers.Count, usersWithImages, usersWithoutImages);
                
                return Json(new { 
                    success = true, 
                    totalTestUsers = testUsers.Count,
                    usersWithImages = usersWithImages,
                    usersWithoutImages = usersWithoutImages,
                    userDetails = userDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking test users");
                return Json(new { success = false, message = "Error checking test users: " + ex.Message });
            }
        }
    }

    public class BanUserRequest
    {
        public string? Reason { get; set; }
    }

    public class NotificationRequest
    {
        public string Message { get; set; } = "";
        public string Target { get; set; } = "";
    }

    public class UnbanUserRequest
    {
        public int UserId { get; set; }
    }
}
