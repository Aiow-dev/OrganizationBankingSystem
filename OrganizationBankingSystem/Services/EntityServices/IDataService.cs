using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public interface IDataService<T>
    {
        Task<T> Create(T entity);
    }
}
