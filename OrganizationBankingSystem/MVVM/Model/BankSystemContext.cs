using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace BankSystemModel
{
    public class BankSystemContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BankUser> BankUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["BankSystemDatabase"].ConnectionString);
        }
    }
}
