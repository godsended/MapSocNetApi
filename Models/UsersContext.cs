using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace BumberAPI.Models
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            
        }
    }
}
