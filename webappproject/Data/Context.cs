using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using webappproject.Models;
using webappproject;

namespace webappproject.Data
{

    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost; Database=webappproject; Trusted_Connection=true; TrustServerCertificate=true;");
        }

        public Context(DbContextOptions<Context> options) : base(options){
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<BannedUser> BannedUsers { get; set; }

    }

}
