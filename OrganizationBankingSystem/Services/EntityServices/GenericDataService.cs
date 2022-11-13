using Microsoft.EntityFrameworkCore.ChangeTracking;
using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public class GenericDataService<T> : IDataService<T> where T : class
    {
        private readonly BankSystemContextFactory _contextFactory;

        public GenericDataService(BankSystemContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using BankSystemContext context = _contextFactory.CreateDbContext();
            EntityEntry<T> createdResult = await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }
    }
}
