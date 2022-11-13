using OrganizationBankingSystem.MVVM.Model;

namespace OrganizationBankingSystem.Core.State.Authenticators
{
    public interface IAuthenticator
    {
        BankUser CurrentBankUser { get; }

        bool IsLogged { get; }
    }
}
