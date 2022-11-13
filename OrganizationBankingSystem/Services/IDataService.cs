using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services
{
    public interface IDataService<T>
    {
        Task<T> Create(T entity);
    }
}
