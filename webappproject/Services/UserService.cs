using webappproject.Models;

namespace webappproject.Services
{
    public class UserService:Repository<User>
    {
        //UserService _rolService = new UserService();
        RolService _rolService = new RolService();
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
            var rolId = _rolService.Get(f => f.Name == "Admin").FirstOrDefault().Id;
           
            return Get(x => x.Email == email && x.Password == password && x.RolId == rolId).Any();
        }

    }
}
