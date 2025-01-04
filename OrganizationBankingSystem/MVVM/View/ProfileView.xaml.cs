using System.Windows.Controls;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.MVVM.View.Dialogs;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        private int _userId;
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
                _userId = user.Id;
                UserText.Text = $"Профиль пользователя {bankUser.Login}";
                LastNameText.Text = user.LastName;
                FirstNameText.Text = user.FirstName;
                PatronymicText.Text = user.Patronymic;
                PhoneText.Text = user.Phone;
                LoginText.Text = bankUser.Login;
            }
        }

        private void ShowChangePhoneForm(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePhoneNumberDialog changePhoneNumberDialog = new ChangePhoneNumberDialog(_userId);
            changePhoneNumberDialog.OnChangePhoneNumber += value => PhoneText.Text = value;
            changePhoneNumberDialog.ShowDialog();
        }
    }
}
