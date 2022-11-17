using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.AuthenticationServices;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Core.State.Authenticators
{
    public class Authenticator : IAuthenticator
    {
        private readonly IAuthenticationService _authenticationService;

        public BankUser CurrentBankUser { get; private set; }

        public bool IsLogged => CurrentBankUser != null;

        public Authenticator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<RegistrationResult> Register(string lastName, string firstName, string patronymic, string phone, string login, string password, string confirmPassword)
        {
            return await _authenticationService.Register(lastName, firstName, patronymic, phone, login, password, confirmPassword);
        }

        public async Task<bool> Login(string login, string password)
        {
            CurrentBankUser = await _authenticationService.Login(login, password);

            return CurrentBankUser != null;
        }

        public void Logout()
        {
            CurrentBankUser = null;
        }
    }
}
