using Microsoft.EntityFrameworkCore;
using webappproject.Models;
using webappproject;

namespace webappproject.Data
{

    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options){
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }

    }

}
