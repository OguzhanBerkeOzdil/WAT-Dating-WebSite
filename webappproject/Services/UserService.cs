using webappproject.Models;
using Microsoft.EntityFrameworkCore;

namespace webappproject.Services
{
    public class UserService : Repository<User>
    {
        private readonly RolService _rolService;
        
        public UserService(Context context, RolService rolService) : base(context)
        {
            _rolService = rolService;
        }
        
        public bool CheckEmail(string email)
        {
            return GetAll().Any(x => x.Email.Equals(email));
        }

        public bool IsCustomer(string email, string password)
        {
            return Get(x => x.Email == email && x.Password == password).Any();
        }

        public bool IsAdmin(string email, string password)
        {
            var adminRole = _rolService.Get(f => f.Name == "Admin").FirstOrDefault();
            if (adminRole == null) return false;
            
            var rolId = adminRole.Id;
            return Get(x => x.Email == email && x.Password == password && x.RolId == rolId).Any();
        }

        // Admin Dashboard Methods
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await context.Users.CountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await context.Users
                .Where(u => u.IsActive == true)
                .CountAsync();
        }

        public async Task<int> GetRecentRegistrationsCountAsync()
        {
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            return await context.Users
                .Where(u => u.CreatedDate >= thirtyDaysAgo)
                .CountAsync();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.Users
                .Include(u => u.Rol)
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await GetUserByIdAsync(id);
                if (user != null)
                {
                    context.Users.Remove(user);
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user != null)
                {
                    user.IsActive = !user.IsActive;
                    await UpdateUserAsync(user);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
