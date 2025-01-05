using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrganizationBankingSystem.MVVM.Model;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public enum DbResult
    {
        Success,
        MatchingValue,
        NotFound,
        PasswordDoNotMatch
    }
    public class BankUserDataService : IBankUserService
    {
        private readonly BankSystemContextFactory _contextFactory;
        private readonly IPasswordHasher _passwordHasher;

        public BankUserDataService(BankSystemContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _passwordHasher = new PasswordHasher();
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

        public async Task<User> GetUserById(int userId)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync(item => item.Id == userId);
        }

        public async Task<DbResult> ChangeUserPhone(int userId, string phone)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            User user = await context.Users.FirstOrDefaultAsync(item => item.Id == userId);
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

        public async Task<DbResult> ChangePassword(int bankUserId, string currentPassword, string newPassword)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            BankUser bankUser = await context.BankUsers.FirstOrDefaultAsync(item => item.Id == bankUserId);
            if (bankUser == null)
            {
                return DbResult.NotFound;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(bankUser.Password, currentPassword);
            if (passwordResult != PasswordVerificationResult.Success)
            {
                return DbResult.PasswordDoNotMatch;
            }

            string hashedNewPassword = _passwordHasher.HashPassword(newPassword);
            bankUser.Password = hashedNewPassword;
            context.SaveChanges();

            return DbResult.Success;
        }

        public async Task<DbResult> Delete(int bankUserId)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            BankUser bankUser = await context.BankUsers.FirstOrDefaultAsync(item => item.Id == bankUserId);
            if (bankUser == null)
            {
                return DbResult.NotFound;
            }
            User user = await context.Users.FirstOrDefaultAsync(item => item.Id == bankUser.UserId);

            context.BankUsers.Remove(bankUser);
            context.Users.Remove(user);
            context.SaveChanges();
            return DbResult.Success;
        }
    }
}
