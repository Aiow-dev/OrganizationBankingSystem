using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services
{
    public interface IBankUserService : IDataService<BankUser>
    {
        Task<BankUser> GetByLogin(string login);
    }
}
