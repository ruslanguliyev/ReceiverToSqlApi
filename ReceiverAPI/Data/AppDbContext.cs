using Microsoft.EntityFrameworkCore;
using ReceiverAPI.Models;

namespace ReceiverAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  
    }
}
