using Microsoft.AspNet.Identity;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.Services.EntityServices;
using System.Threading.Tasks;

namespace OrganizationBankingSystem.Services.AuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IBankUserService _bankUserService;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticationService(IBankUserService bankUserService)
        {
            _bankUserService = bankUserService;
            _passwordHasher = new PasswordHasher();
        }

        public async Task<BankUser> Login(string login, string password)
        {
            BankUser bankUser = await _bankUserService.GetByLogin(login);
            if (bankUser == null)
            {
                return null;
            }

            PasswordVerificationResult passwordResult = _passwordHasher.VerifyHashedPassword(bankUser.Password, password);

            if (passwordResult != PasswordVerificationResult.Success)
            {
                return null;
            }

            return bankUser;
        }

        public async Task<RegistrationResult> Register(string lastName, string firstName, string patronymic, string phone, string login, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                return RegistrationResult.PasswordDoNotMatch;
            }

            string hashedPassword = _passwordHasher.HashPassword(password);

            User user = new()
            {
                LastName = lastName,
                FirstName = firstName,
                Patronymic = patronymic,
                Phone = phone,
            };

            BankUser bankUser = new()
            {
                Login = login,
                Password = hashedPassword,
                User = user
            };

            await _bankUserService.Create(bankUser);

            return RegistrationResult.Success;
        }
    }
}
