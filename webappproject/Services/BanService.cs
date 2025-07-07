using webappproject.Models;
using Microsoft.EntityFrameworkCore;

namespace webappproject.Services
{
    public class BanService : Repository<BannedUser>
    {
        public BanService(Context context) : base(context)
        {
        }
        
        public bool IsBanned(string email)
        {
            return Get(x => x.Email == email).Any();
        }

        public async Task<int> GetBannedUsersCountAsync()
        {
            return await context.BannedUsers.CountAsync();
        }

        public async Task<List<BannedUser>> GetAllBannedUsersAsync()
        {
            return await context.BannedUsers
                .OrderByDescending(b => b.BanDate)
                .ToListAsync();
        }

        public async Task<List<int>> GetBannedUserIdsAsync()
        {
            return await context.BannedUsers
                .Join(context.Users, 
                      banned => banned.Email, 
                      user => user.Email, 
                      (banned, user) => user.Id)
                .ToListAsync();
        }

        public async Task<List<BannedUser>> GetBannedUsersAsync()
        {
            return await context.BannedUsers
                .OrderByDescending(b => b.BanDate)
                .ToListAsync();
        }

        public Task<bool> BanUserAsync(string email, string reason, string bannedBy)
        {
            try
            {
                if (IsBanned(email)) return Task.FromResult(false);

                var bannedUser = new BannedUser
                {
                    Email = email,
                    Reason = reason,
                    BanDate = DateTime.Now,
                    BannedBy = bannedBy
                };

                Add(bannedUser);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<bool> UnbanUserAsync(string email)
        {
            try
            {
                var bannedUser = Get(x => x.Email == email).FirstOrDefault();
                if (bannedUser != null)
                {
                    context.BannedUsers.Remove(bannedUser);
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

        public async Task<bool> BanUserAsync(int userId, string reason, string bannedBy)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null) return false;

                if (IsBanned(user.Email)) return false;

                var bannedUser = new BannedUser
                {
                    Email = user.Email,
                    Reason = reason,
                    BanDate = DateTime.Now,
                    BannedBy = bannedBy,
                    UserId = userId
                };

                Add(bannedUser);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<BannedUser?> GetBanDetailsAsync(int userId)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null) return null;

                return Get(x => x.Email == user.Email).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> BanUserAsync(BannedUser bannedUser)
        {
            try
            {
                // Check if user is already banned
                var user = await context.Users.FindAsync(bannedUser.UserId);
                if (user == null) return false;

                var existingBan = Get(x => x.Email == user.Email).FirstOrDefault();
                if (existingBan != null) return false;

                bannedUser.Email = user.Email;
                context.BannedUsers.Add(bannedUser);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnbanUserAsync(int bannedUserId)
        {
            try
            {
                var bannedUser = await context.BannedUsers.FindAsync(bannedUserId);
                if (bannedUser == null) return false;

                context.BannedUsers.Remove(bannedUser);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<BannedUser?> GetBannedUserByUserIdAsync(int userId)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null) return null;

                return await context.BannedUsers
                    .FirstOrDefaultAsync(b => b.Email == user.Email);
            }
            catch
            {
                return null;
            }
        }
    }
}