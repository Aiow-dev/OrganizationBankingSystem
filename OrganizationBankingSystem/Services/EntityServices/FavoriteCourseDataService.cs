using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrganizationBankingSystem.MVVM.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public class FavoriteCourseDataService
    {
        private readonly BankSystemContextFactory _contextFactory;

        public FavoriteCourseDataService(BankSystemContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<FavoriteCourse> Create(FavoriteCourse favoriteCourse)
        {
            MessageBox.Show("FavoriteCourse");
            using BankSystemContext context = _contextFactory.CreateDbContext();
            EntityEntry<FavoriteCourse> createdResult = await context.FavoriteCourses.AddAsync(favoriteCourse);
            MessageBox.Show("Добавление");
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<List<FavoriteCourse>> GetByBankUser(BankUser bankUser)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();

            return await context.FavoriteCourses.Include(item => item.BankUser).Where(item => item.BankUserId == bankUser.Id).ToListAsync();
        }
    }
}
