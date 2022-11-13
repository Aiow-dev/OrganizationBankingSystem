using Microsoft.AspNet.Identity;
using OrganizationBankingSystem.MVVM.Model;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDataService<BankUser> _bankUserService;

        public AuthenticationService(IDataService<BankUser> bankUserService)
        {
            _bankUserService = bankUserService;
        }

        public async Task<BankUser> Login(string login, string password)
        {
            return null;
        }

        public async Task<bool> Register(string lastName, string firstName, string patronymic, string phone, string login, string password, string confirmPassword)
        {
            bool success = false;

            if (password == confirmPassword)
            {
                IPasswordHasher hasher = new PasswordHasher();

                string hashedPassword = hasher.HashPassword(password);

                BankUser bankUser = new()
                {
                    Login = login,
                    Password = hashedPassword
                };

                User user = new()
                {
                    LastName = lastName,
                    FirstName = firstName,
                    Patronymic = patronymic,
                    Phone = phone,
                    BankUser = bankUser
                };

                await _bankUserService.Create(bankUser);
            }

            return success;
        }
    }
}
