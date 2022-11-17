using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.EntityServices
{
    public interface IBankUserService : IDataService<BankUser>
    {
        Task<BankUser> GetByLogin(string login);
    }
}
