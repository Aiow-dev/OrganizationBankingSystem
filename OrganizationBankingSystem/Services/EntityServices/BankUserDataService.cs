using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public class BankUserDataService : IBankUserService
    {
        private readonly BankSystemContextFactory _contextFactory;

        public BankUserDataService(BankSystemContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<BankUser> Create(BankUser bankUser)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            EntityEntry<BankUser> createdResult = await context.BankUsers.AddAsync(bankUser);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<BankUser> GetByLogin(string login)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();

            return await context.BankUsers.FirstOrDefaultAsync(item => item.Login == login);
        }

        public async Task<User> GetUserById(int id)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync(item => item.Id == id);
        }
    }
}
