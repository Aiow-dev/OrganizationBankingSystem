using System.Windows.Controls;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.MVVM.Model;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();

            LoadViewState();
        }

        public void LoadViewState()
        {
            BankUser bankUser = AuthenticatorState.authenticator.CurrentBankUser;
            if (bankUser != null)
            {
                User user = bankUser.User;
                UserText.Text = $"Профиль пользователя {bankUser.Login}";
                LastNameText.Text = user.LastName;
                FirstNameText.Text = user.FirstName;
                PatronymicText.Text = user.Patronymic;
                PhoneText.Text = user.Phone;
                LoginText.Text = bankUser.Login;
            }
        }
    }
}
