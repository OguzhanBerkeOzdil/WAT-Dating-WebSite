using webappproject.Models;

namespace webappproject.Services
{
    public class BanService : Repository<BannedUser>
    {
        public bool IsBanned(string email)
        {
            return Get(x => x.Email == email).Any();
        }
    }
}