using Microsoft.EntityFrameworkCore;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankSystemContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<BankUser> BankUsers { get; set; }

        public BankSystemContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
