using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.AuthenticationServices;
using OrganizationBankingSystem.Services.EntityServices;

namespace OrganizationBankingSystem.Core.State.Authenticators
{
    public class AuthenticatorState
    {
        public static readonly IAuthenticator authenticator = new Authenticator(new AuthenticationService(new BankUserDataService(new BankSystemContextFactory())));
    }
}
