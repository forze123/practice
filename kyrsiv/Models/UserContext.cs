using kyrsiv.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Messeng> Messengs {get; set;}
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}