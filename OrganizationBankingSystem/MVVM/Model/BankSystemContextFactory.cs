using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Configuration;

namespace OrganizationBankingSystem.MVVM.Model
{
    public class BankSystemContextFactory : IDesignTimeDbContextFactory<BankSystemContext>
    {
        public BankSystemContext CreateDbContext(string[] args = null)
        {
            DbContextOptionsBuilder<BankSystemContext> optionsBuilder = new();

            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["BankSystemDatabase"].ConnectionString);

            return new BankSystemContext(optionsBuilder.Options);
        }
    }
}
