using webappproject.Models;

namespace webappproject.Services
{
    public class RolService : Repository<Rol>
    {
        public RolService(Context context) : base(context)
        {
        }
    }
}
