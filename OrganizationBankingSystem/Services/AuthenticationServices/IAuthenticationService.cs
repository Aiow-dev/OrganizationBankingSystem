using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        Task<bool> Register(string lastName, string firstName, string patronymic, string phone, string login, string password, string confirmPassword);
        Task<BankUser> Login(string login, string password);
    }
}
