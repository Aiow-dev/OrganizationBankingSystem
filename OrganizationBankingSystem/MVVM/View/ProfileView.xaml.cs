using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OrganizationBankingSystem.Core.Notifications;
using OrganizationBankingSystem.Core.State.Authenticators;
using OrganizationBankingSystem.MVVM.Model;
using OrganizationBankingSystem.MVVM.View.Dialogs;
using OrganizationBankingSystem.Services.EntityServices;

namespace OrganizationBankingSystem.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        private int _userId;
        private int _bankUserId;
        private DbResult _result;
        private readonly BankUserDataService _bankUserDataService;
        public ProfileView()
        {
            InitializeComponent();
            _bankUserDataService = new(new BankSystemContextFactory());
            LoadViewState();
        }

        public void LoadViewState()
        {
            BankUser bankUser = AuthenticatorState.authenticator.CurrentBankUser;
            if (bankUser != null)
            {
                _bankUserId = bankUser.Id;
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

        private void ShowChangePasswordForm(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangePasswordDialog changePhoneNumberDialog = new ChangePasswordDialog(_bankUserId);
            changePhoneNumberDialog.ShowDialog();
        }

        private async Task DeleteAccountResult()
        {
            _result = await Task.Run(() => _bankUserDataService.Delete(_bankUserId));
        }

        private async void ShowDeleteAccountForm(object sender, System.Windows.RoutedEventArgs e)
        {
            bool isConfirm = false;
            QuestionDialog changePhoneNumberDialog = new QuestionDialog("Удаление учетной записи", "Вы уверены, что хотите удалить учетную запись? После удаления учетную запись невозможно будет восстановить! Внимание! Приложение будет перезагружено");
            changePhoneNumberDialog.OnClickButton += value => isConfirm = value;
            changePhoneNumberDialog.ShowDialog();

            if (isConfirm)
            {
                await DeleteAccountResult();
                if (_result == DbResult.NotFound)
                {
                    NotificationManager.mainNotifier.ShowErrorPropertyMessage("Ошибка. Пользователь не найден");
                    return;
                }
                Application.Current.Shutdown();
                System.Windows.Forms.Application.Restart();
            }
        }
    }
}
