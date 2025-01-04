using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrganizationBankingSystem.MVVM.Model;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public enum DbResult
    {
        Success,
        MatchingValue,
        NotFound
    }
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

        public async Task<DbResult> ChangeUserPhone(int id, string phone)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            User user = await context.Users.FirstOrDefaultAsync(item => item.Id == id);
            if (user == null)
            {
                return DbResult.NotFound;
            }
            if (user.Phone == phone)
            {
                return DbResult.MatchingValue;
            }
            user.Phone = phone;
            context.SaveChanges();

            return DbResult.Success;
        }
    }
}
