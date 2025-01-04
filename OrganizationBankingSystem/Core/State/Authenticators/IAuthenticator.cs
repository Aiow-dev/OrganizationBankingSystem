using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.AuthenticationServices;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Core.State.Authenticators
{
    public interface IAuthenticator
    {
        BankUser CurrentBankUser { get; }

        bool IsLogged { get; }

        Task<RegistrationResult> Register(string lastName, string firstName, string patronymic, string phone, string login, string password, string confirmPassword);
        
        Task<bool> Login(string login, string password);
        void UpdateUserPhone(string phone);

        void Logout();
    }
}
